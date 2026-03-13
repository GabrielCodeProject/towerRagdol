using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using RagdollRealms.Systems.Ragdoll;

namespace RagdollRealms.Systems.Combat
{
    public class MeleeCombatController : MonoBehaviour
    {
        [SerializeField] private float _baseDamage = 10f;
        [SerializeField] private float _velocityDamageMultiplier = 2f;
        [SerializeField] private float _minVelocityToDamage = 1f;
        [SerializeField] private float _hitCooldown = 0.3f;
        [SerializeField] private float _knockbackForceMultiplier = 5f;

        private Rigidbody _rigidbody;
        private float _lastHitTime;
        private IEventBus _eventBus;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (ServiceLocator.Instance != null && ServiceLocator.Instance.TryGet<IEventBus>(out var bus))
                _eventBus = bus;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Time.time - _lastHitTime < _hitCooldown)
                return;

            float velocity = _rigidbody.linearVelocity.magnitude;

            if (velocity < _minVelocityToDamage)
                return;

            float damage = _baseDamage + velocity * _velocityDamageMultiplier;

            var receiver = collision.gameObject.GetComponentInParent<RagdollForceReceiver>();
            Vector3 force = Vector3.zero;
            int targetId = collision.gameObject.GetInstanceID();

            if (receiver != null)
            {
                force = -collision.contacts[0].normal * velocity * _knockbackForceMultiplier;

                var ragdollController = collision.gameObject.GetComponentInParent<IRagdollController>();
                if (ragdollController != null)
                    force *= (1f - ragdollController.Config.ArmorKnockbackReduction);

                receiver.ApplyForce(force, ForceType.Impact, collision.rigidbody);
            }

            _eventBus.Publish(new OnMeleeHit(
                gameObject.GetInstanceID(),
                targetId,
                damage,
                collision.contacts[0].point,
                force
            ));

            _lastHitTime = Time.time;
        }
    }
}
