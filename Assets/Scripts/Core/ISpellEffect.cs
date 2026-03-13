using UnityEngine;

namespace RagdollRealms.Core
{
    public interface ISpellEffect
    {
        void Cast(GameObject caster, Vector3 targetPosition, int spellLevel);
        void OnHit(GameObject target, float damage);
        void OnExpire();
    }
}
