using UnityEngine;

namespace RagdollRealms.Core
{
    public interface IProjectileFactory
    {
        Component SpawnProjectile(
            GameObject prefab,
            Vector3 position,
            Quaternion rotation,
            float damage,
            float speed,
            float knockback,
            ForceType forceType);
    }
}
