using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using RagdollRealms.Core.StateMachine;

namespace RagdollRealms.Systems.Ragdoll
{
    public class RagdollRecoveryController : MonoBehaviour
    {
        private StateMachine _stateMachine;
        private IRagdollController _controller;
        private AnimationFollower _animFollower;
        private RagdollForceReceiver _forceReceiver;
        private IEventBus _eventBus;
        private float _vitality = 1f;

        private void Start()
        {
            _controller = GetComponent<IRagdollController>();
            _animFollower = GetComponent<AnimationFollower>();
            _forceReceiver = GetComponent<RagdollForceReceiver>();
            if (ServiceLocator.Instance != null && ServiceLocator.Instance.TryGet<IEventBus>(out var bus))
                _eventBus = bus;

            _stateMachine = new StateMachine();
            _stateMachine.AddState(new ActiveState(this));
            _stateMachine.AddState(new DownedState(this));
            _stateMachine.AddState(new RecoveringState(this));
            _stateMachine.SetState<ActiveState>();

            SubscribeEvents();
        }

        private void Update()
        {
            _stateMachine?.Update();
        }

        public void TriggerRagdoll()
        {
            _stateMachine.SetState<DownedState>();
        }

        public void SetVitality(float v)
        {
            _vitality = Mathf.Clamp01(v);
        }

        public bool IsRecovering => _stateMachine != null && _stateMachine.IsInState<RecoveringState>();

        private void SubscribeEvents()
        {
            _eventBus?.Subscribe<OnRagdollActivated>(OnRagdollActivatedHandler);
        }

        private void OnDisable()
        {
            _eventBus?.Unsubscribe<OnRagdollActivated>(OnRagdollActivatedHandler);
        }

        private void OnDestroy()
        {
            _eventBus?.Unsubscribe<OnRagdollActivated>(OnRagdollActivatedHandler);
        }

        private void OnRagdollActivatedHandler(OnRagdollActivated evt)
        {
            if (evt.EntityId == gameObject.GetInstanceID())
            {
                TriggerRagdoll();
            }
        }

        private class ActiveState : IState
        {
            private readonly RagdollRecoveryController _parent;

            public ActiveState(RagdollRecoveryController parent)
            {
                _parent = parent;
            }

            public void Enter()
            {
                _parent._animFollower.SetEnabled(true);
                _parent._animFollower.BlendWeight = 1f;
            }

            public void Update() { }

            public void Exit() { }
        }

        private class DownedState : IState
        {
            private readonly RagdollRecoveryController _parent;
            private float _downedTime;

            public DownedState(RagdollRecoveryController parent)
            {
                _parent = parent;
            }

            public void Enter()
            {
                _downedTime = Time.time;
            }

            public void Update()
            {
                bool delayElapsed = Time.time - _downedTime >= _parent._controller.Config.RecoveryDelay;
                bool hasVitality = _parent._vitality >= _parent._controller.Config.MinVitalityToRecover;

                if (delayElapsed && hasVitality)
                {
                    _parent._stateMachine.SetState<RecoveringState>();
                }
            }

            public void Exit() { }
        }

        private class RecoveringState : IState
        {
            private readonly RagdollRecoveryController _parent;
            private float _recoveryStartTime;

            public RecoveringState(RagdollRecoveryController parent)
            {
                _parent = parent;
            }

            public void Enter()
            {
                _parent._forceReceiver.SetAnimatorEnabled(true);
                _parent._animFollower.SetEnabled(true);
                _parent._animFollower.BlendWeight = 0f;
                _recoveryStartTime = Time.time;
                _parent._eventBus?.Publish(new OnRagdollRecoveryStarted(_parent.gameObject.GetInstanceID()));
            }

            public void Update()
            {
                float t = (Time.time - _recoveryStartTime) / Mathf.Max(_parent._controller.Config.RecoveryDuration, 0.001f);
                t = Mathf.Clamp01(t);

                _parent._animFollower.BlendWeight = t;
                _parent._controller.SetJointSpringMultiplier(t * _parent._controller.Config.GetUpSpringMultiplier);

                if (t >= 1f)
                {
                    _parent._stateMachine.SetState<ActiveState>();
                }
            }

            public void Exit()
            {
                _parent._controller.SetJointSpringMultiplier(1f);
                _parent._forceReceiver.DeactivateRagdoll();
                _parent._eventBus?.Publish(new OnRagdollRecovered(_parent.gameObject.GetInstanceID()));
            }
        }
    }
}
