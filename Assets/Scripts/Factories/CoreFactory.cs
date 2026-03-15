using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Content;

namespace RagdollRealms.Factories
{
    public sealed class CoreFactory : MonoBehaviour, ICoreFactory
    {
        private IPoolManager _poolManager;
        private CoreDefinition _config;

        private void Awake()
        {
            ServiceLocator.Instance.Register<ICoreFactory>(this);
        }

        private void Start()
        {
            _poolManager = ServiceLocator.Instance.Get<IPoolManager>();

            var registry = ServiceLocator.Instance.Get<IContentRegistry<CoreDefinition>>();
            var allConfigs = registry.GetAll();
            if (allConfigs.Count > 0)
                _config = allConfigs[0];
        }

        public GameObject SpawnCore(Vector3 position)
        {
            if (_config == null || _config.CorePrefab == null)
            {
                Debug.LogError("[CoreFactory] No CoreDefinition or CorePrefab configured!");
                return null;
            }

            var spawnable = _config.CorePrefab.GetComponent<ISpawnable>();
            if (spawnable == null)
            {
                Debug.LogError("[CoreFactory] CorePrefab missing ISpawnable component!");
                return null;
            }

            var core = _poolManager.Spawn((MonoBehaviour)spawnable, position, Quaternion.identity);
            return core?.gameObject;
        }

        private void OnDestroy()
        {
            if (ServiceLocator.Instance != null)
                ServiceLocator.Instance.Unregister<ICoreFactory>();
        }
    }
}
