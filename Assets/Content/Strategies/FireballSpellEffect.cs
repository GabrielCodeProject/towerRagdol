using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core;

namespace RagdollRealms.Content.Strategies
{
    [CreateAssetMenu(fileName = "FireballEffect", menuName = "Ragdoll Realms/Strategies/Fireball")]
    public class FireballSpellEffect : ScriptableObject, ISpellEffect
    {
        [SerializeField] private float _explosionRadius = 5f;
        [SerializeField] private float _explosionForce = 30f;
        [SerializeField] private float _upwardsModifier = 1f;

        public void Cast(GameObject caster, Vector3 targetPosition, int spellLevel)
        {
            if (ServiceLocator.Instance.TryGet<IProjectileFactory>(out _))
            {
                Debug.Log($"[FireballSpellEffect] Projectile factory available — casting system will handle projectile spawn toward {targetPosition}");
            }
            else
            {
                Debug.Log($"[FireballSpellEffect] Cast toward {targetPosition} at level {spellLevel} — no projectile factory registered");
            }
        }

        public void OnHit(GameObject target, float damage)
        {
            Vector3 explosionCenter = target.transform.position;
            Collider[] hits = Physics.OverlapSphere(explosionCenter, _explosionRadius);

            var processed = new HashSet<GameObject>();

            foreach (var collider in hits)
            {
                GameObject root = collider.transform.root.gameObject;

                if (!processed.Add(root))
                    continue;

                var controller = root.GetComponentInChildren<IRagdollController>();
                if (controller == null)
                    continue;

                Vector3 direction = (collider.transform.position - explosionCenter).normalized;
                if (direction == Vector3.zero) direction = Vector3.up;
                Vector3 force = direction * _explosionForce + Vector3.up * _upwardsModifier;

                foreach (var body in controller.AllBodies)
                {
                    body.AddExplosionForce(force.magnitude, explosionCenter, _explosionRadius);
                }

                if (force.magnitude > controller.Config.RagdollForceThreshold)
                {
                    controller.IsRagdolling = true;

                    var monoBehaviour = controller as MonoBehaviour;
                    if (monoBehaviour != null)
                    {
                        var eventBus = ServiceLocator.Instance.TryGet<IEventBus>(out var bus) ? bus : null;
                        eventBus?.Publish(new Core.Events.OnRagdollActivated(
                            monoBehaviour.gameObject.GetInstanceID(),
                            monoBehaviour.transform.position));
                    }
                }
            }
        }

        public void OnExpire()
        {
            // No-op — fireball effect is instantaneous on hit
        }
    }
}
