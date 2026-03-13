using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core;

namespace RagdollRealms.Content.Strategies
{
    [CreateAssetMenu(fileName = "LightningEffect", menuName = "Ragdoll Realms/Strategies/Lightning")]
    public class LightningSpellEffect : ScriptableObject, ISpellEffect
    {
        [SerializeField] private float _chainRange = 8f;
        [SerializeField] private int _maxChainTargets = 3;
        [SerializeField] private float _stunDuration = 1f;
        [SerializeField] private float _chainDamageFalloff = 0.7f;

        public float StunDuration => _stunDuration;
        public float ChainDamageFalloff => _chainDamageFalloff;
        public int MaxChainTargets => _maxChainTargets;
        public float ChainRange => _chainRange;

        public void Cast(GameObject caster, Vector3 targetPosition, int spellLevel)
        {
            // No-op — lightning applies effect directly on hit
        }

        public void OnHit(GameObject target, float damage)
        {
            var primaryController = target.GetComponentInParent<IRagdollController>();
            if (primaryController != null)
            {
                primaryController.SetJointSpringMultiplier(0f);
                Debug.Log($"[LightningSpellEffect] Stunned primary target {target.name} for {_stunDuration}s");
            }

            Collider[] hits = Physics.OverlapSphere(target.transform.position, _chainRange);

            var chainControllers = new List<IRagdollController>();
            var processed = new HashSet<GameObject>();
            processed.Add(target.transform.root.gameObject);

            foreach (var collider in hits)
            {
                if (chainControllers.Count >= _maxChainTargets)
                    break;

                GameObject root = collider.transform.root.gameObject;
                if (!processed.Add(root))
                    continue;

                var chainController = root.GetComponentInChildren<IRagdollController>();
                if (chainController == null)
                    continue;

                chainControllers.Add(chainController);
            }

            for (int i = 0; i < chainControllers.Count; i++)
            {
                var chainController = chainControllers[i];
                float chainDamage = damage * Mathf.Pow(_chainDamageFalloff, i + 1);

                chainController.SetJointSpringMultiplier(0f);

                Vector3 impactDirection = (Vector3.up + Random.insideUnitSphere * 0.3f).normalized;
                foreach (var body in chainController.AllBodies)
                {
                    body.AddForce(impactDirection * 5f, ForceMode.Impulse);
                }

                Debug.Log($"[LightningSpellEffect] Chain {i + 1}: stunned target, chain damage = {chainDamage:F1}");
            }
        }

        public void OnExpire()
        {
            // Stateless SO — the casting system tracks affected targets and calls
            // controller.SetJointSpringMultiplier(1f) when StunDuration elapses.
            // Exposed StunDuration property lets the casting system read the timing.
        }
    }
}
