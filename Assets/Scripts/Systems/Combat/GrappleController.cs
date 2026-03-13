using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using RagdollRealms.Core.StateMachine;
using RagdollRealms.Systems.Ragdoll;

namespace RagdollRealms.Systems.Combat
{
    public class GrappleController : MonoBehaviour
    {
        [SerializeField] private float _grabRange = 2f;
        [SerializeField] private float _grabBreakForce = 500f;
        [SerializeField] private float _throwForce = 30f;
        [SerializeField] private float _punchDamage = 15f;
        [SerializeField] private float _punchCooldown = 0.5f;
        [SerializeField] private Transform _grabPoint;

        private StateMachine _stateMachine;
        private FixedJoint _fixedJoint;
        private Rigidbody _grabbedBody;
        private RagdollForceReceiver _grabbedReceiver;
        private IEventBus _eventBus;
        private float _lastPunchTime;

        private void Start()
        {
            if (ServiceLocator.Instance != null && ServiceLocator.Instance.TryGet<IEventBus>(out var bus))
                _eventBus = bus;
            _stateMachine = new StateMachine();
            _stateMachine.AddState(new IdleState(this));
            _stateMachine.AddState(new GrabbingState(this));
            _stateMachine.AddState(new HoldingState(this));
            _stateMachine.AddState(new ThrowingState(this));
            _stateMachine.SetState<IdleState>();
        }

        private void Update()
        {
            _stateMachine?.Update();
        }

        private void OnDisable()
        {
            if (_fixedJoint != null)
            {
                Object.Destroy(_fixedJoint);
            }
            _stateMachine?.SetState<IdleState>();
        }

        public void TryGrab()
        {
            if (_stateMachine.IsInState<IdleState>())
            {
                _stateMachine.SetState<GrabbingState>();
            }
        }

        public void Release()
        {
            if (_fixedJoint != null)
            {
                Object.Destroy(_fixedJoint);
            }

            if (_grabbedBody != null)
            {
                _eventBus.Publish(new OnGrappleReleased(
                    gameObject.GetInstanceID(),
                    _grabbedBody.gameObject.GetInstanceID(),
                    Vector3.zero));
            }

            _stateMachine.SetState<IdleState>();
        }

        public void Throw()
        {
            if (_stateMachine.IsInState<HoldingState>())
            {
                _stateMachine.SetState<ThrowingState>();
            }
        }

        public void Punch()
        {
            if (!_stateMachine.IsInState<HoldingState>()) return;
            if (_grabbedBody == null || _grabbedReceiver == null) return;
            if (Time.time - _lastPunchTime < _punchCooldown) return;

            Vector3 punchForce = (_grabbedBody.position - _grabPoint.position).normalized * _punchDamage;
            _grabbedReceiver.ApplyForce(punchForce, ForceType.Impact, _grabbedBody);
            _lastPunchTime = Time.time;
        }

        public bool IsHolding => _stateMachine.IsInState<HoldingState>();

        private class IdleState : IState
        {
            private readonly GrappleController _parent;

            public IdleState(GrappleController parent)
            {
                _parent = parent;
            }

            public void Enter()
            {
                _parent._fixedJoint = null;
                _parent._grabbedBody = null;
                _parent._grabbedReceiver = null;
            }

            public void Update() { }

            public void Exit() { }
        }

        private class GrabbingState : IState
        {
            private readonly GrappleController _parent;

            public GrabbingState(GrappleController parent)
            {
                _parent = parent;
            }

            public void Enter()
            {
                Collider[] colliders = Physics.OverlapSphere(_parent._grabPoint.position, _parent._grabRange);

                foreach (var hitCollider in colliders)
                {
                    RagdollForceReceiver receiver = hitCollider.GetComponentInParent<RagdollForceReceiver>();
                    if (receiver == null) continue;
                    if (receiver.gameObject == _parent.gameObject) continue;
                    if (receiver.transform.IsChildOf(_parent.transform)) continue;

                    Rigidbody body = hitCollider.attachedRigidbody != null
                        ? hitCollider.attachedRigidbody
                        : hitCollider.GetComponentInParent<Rigidbody>();

                    if (body == null) continue;

                    _parent._grabbedBody = body;
                    _parent._grabbedReceiver = receiver;

                    if (_parent._grabPoint.GetComponent<Rigidbody>() == null)
                    {
                        _parent._grabPoint.gameObject.AddComponent<Rigidbody>();
                    }

                    _parent._fixedJoint = _parent._grabPoint.gameObject.AddComponent<FixedJoint>();
                    _parent._fixedJoint.connectedBody = _parent._grabbedBody;
                    _parent._fixedJoint.breakForce = _parent._grabBreakForce;

                    _parent._eventBus.Publish(new OnGrappleStarted(
                        _parent.gameObject.GetInstanceID(),
                        _parent._grabbedBody.gameObject.GetInstanceID()));

                    _parent._stateMachine.SetState<HoldingState>();
                    return;
                }

                _parent._stateMachine.SetState<IdleState>();
            }

            public void Update() { }

            public void Exit() { }
        }

        private class HoldingState : IState
        {
            private readonly GrappleController _parent;

            public HoldingState(GrappleController parent)
            {
                _parent = parent;
            }

            public void Enter() { }

            public void Update()
            {
                if (_parent._fixedJoint == null)
                {
                    if (_parent._grabbedBody != null)
                    {
                        _parent._eventBus.Publish(new OnGrappleReleased(
                            _parent.gameObject.GetInstanceID(),
                            _parent._grabbedBody.gameObject.GetInstanceID(),
                            Vector3.zero));
                    }

                    _parent._stateMachine.SetState<IdleState>();
                }
            }

            public void Exit() { }
        }

        private class ThrowingState : IState
        {
            private readonly GrappleController _parent;

            public ThrowingState(GrappleController parent)
            {
                _parent = parent;
            }

            public void Enter()
            {
                Rigidbody cachedBody = _parent._grabbedBody;
                RagdollForceReceiver cachedReceiver = _parent._grabbedReceiver;

                if (_parent._fixedJoint != null)
                {
                    Object.Destroy(_parent._fixedJoint);
                }

                Vector3 throwDir = (_parent.transform.forward * 0.7f + Vector3.up * 0.3f).normalized;
                Vector3 force = throwDir * _parent._throwForce;

                if (cachedReceiver != null)
                {
                    cachedReceiver.ApplyForce(force, ForceType.Impact, cachedBody);
                }

                if (cachedBody != null)
                {
                    _parent._eventBus.Publish(new OnGrappleReleased(
                        _parent.gameObject.GetInstanceID(),
                        cachedBody.gameObject.GetInstanceID(),
                        force));
                }

                _parent._stateMachine.SetState<IdleState>();
            }

            public void Update() { }

            public void Exit() { }
        }
    }
}
