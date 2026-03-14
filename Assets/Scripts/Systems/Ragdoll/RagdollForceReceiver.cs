using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;

namespace RagdollRealms.Systems.Ragdoll
{
    public class RagdollForceReceiver : MonoBehaviour
    {
        private const float MaxForceMagnitude = 10000f;
        private const float MaxExplosionRadius = 100f;

        private IRagdollController _controller;
        private AnimationFollower _animFollower;
        private Animator _animator;
        private IEventBus _eventBus;

        private void Start()
        {
            _controller = GetComponent<IRagdollController>();
            _animFollower = GetComponent<AnimationFollower>();
            _animator = GetComponent<Animator>();
            if (ServiceLocator.Instance != null && ServiceLocator.Instance.TryGet<IEventBus>(out var bus))
                _eventBus = bus;
        }

        public void ApplyForce(Vector3 force, ForceType type, Rigidbody targetBone = null)
        {
            if (_controller == null) return;

            targetBone = targetBone ?? _controller.HipRigidbody;
            if (targetBone == null) return;

            force *= _controller.Config.KnockbackMultiplier;
            force = Vector3.ClampMagnitude(force, MaxForceMagnitude);

            switch (type)
            {
                case ForceType.Impact:
                    targetBone.AddForce(force, ForceMode.Impulse);
                    break;
                case ForceType.Explosion:
                    float explosionRadius = Mathf.Min(force.magnitude * 0.5f, MaxExplosionRadius);
                    foreach (var body in _controller.AllBodies)
                    {
                        body.AddExplosionForce(force.magnitude, targetBone.position, explosionRadius);
                    }
                    break;
                case ForceType.Sustained:
                    targetBone.AddForce(force, ForceMode.Force);
                    break;
            }

            _eventBus?.Publish(new OnRagdollForceApplied(gameObject.GetInstanceID(), force, type));

            if (force.magnitude > _controller.Config.RagdollForceThreshold)
            {
                ActivateRagdoll();
            }
        }

        public void ApplyForceAtPosition(Vector3 force, Vector3 position, ForceType type)
        {
            var nearestBody = FindNearestBody(position);
            ApplyForce(force, type, nearestBody);
        }

        public void ActivateRagdoll()
        {
            if (_controller.IsRagdolling) return;

            _controller.IsRagdolling = true;
            _animFollower?.SetEnabled(false);
            SetAnimatorEnabled(false);
            _eventBus?.Publish(new OnRagdollActivated(gameObject.GetInstanceID(), transform.position));
        }

        public void DeactivateRagdoll()
        {
            _controller.IsRagdolling = false;
            _animFollower?.SetEnabled(true);
            SetAnimatorEnabled(true);
            _eventBus?.Publish(new OnRagdollDeactivated(gameObject.GetInstanceID()));
        }

        public void SetAnimatorEnabled(bool enabled)
        {
            if (_animator != null) _animator.enabled = enabled;
        }

        private Rigidbody FindNearestBody(Vector3 position)
        {
            Rigidbody nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var body in _controller.AllBodies)
            {
                float distance = Vector3.Distance(body.position, position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = body;
                }
            }

            return nearest;
        }
    }
}
