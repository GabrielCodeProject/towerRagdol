using UnityEngine;
using RagdollRealms.Core;

namespace RagdollRealms.Factories
{
    public class ProjectileFactory : MonoBehaviour, IProjectileFactory
    {
        private void Awake()
        {
            ServiceLocator.Instance.Register<IProjectileFactory>(this);
        }

        public Component SpawnProjectile(
            GameObject prefab,
            Vector3 position,
            Quaternion rotation,
            float damage,
            float speed,
            float knockback,
            ForceType forceType)
        {
            var pool = ServiceLocator.Instance.Get<IPoolManager>();
            var proj = pool.Spawn(prefab.GetComponent<MonoBehaviour>(), position, rotation);

            if (proj is IConfigurableProjectile configurable)
                configurable.Configure(damage, speed, knockback, forceType);

            return proj;
        }

        private void OnDestroy()
        {
            if (ServiceLocator.Instance != null)
                ServiceLocator.Instance.Unregister<IProjectileFactory>();
        }
    }
}
