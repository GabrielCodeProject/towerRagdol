using UnityEngine;
using RagdollRealms.Core;

namespace RagdollRealms.Content.Strategies
{
    [CreateAssetMenu(fileName = "IceEffect", menuName = "Ragdoll Realms/Strategies/Ice")]
    public class IceSpellEffect : ScriptableObject, ISpellEffect
    {
        [SerializeField] private float _freezeDuration = 3f;
        [SerializeField] private float _shatterDamageMultiplier = 2f;
        [SerializeField] private float _freezeSpringMultiplier = 10f;

        public float FreezeDuration => _freezeDuration;
        public float ShatterDamageMultiplier => _shatterDamageMultiplier;
        public float FreezeSpringMultiplier => _freezeSpringMultiplier;

        public void Cast(GameObject caster, Vector3 targetPosition, int spellLevel)
        {
            // No-op — ice applies effect directly on hit
        }

        public void OnHit(GameObject target, float damage)
        {
            var controller = target.GetComponentInParent<IRagdollController>();
            if (controller == null)
                return;

            controller.SetJointSpringMultiplier(_freezeSpringMultiplier);
        }

        public void OnExpire()
        {
            // Stateless SO — the casting system tracks affected targets and calls
            // controller.SetJointSpringMultiplier(1f) when FreezeDuration elapses.
            // Exposed FreezeDuration property lets the casting system read the timing.
        }
    }
}
