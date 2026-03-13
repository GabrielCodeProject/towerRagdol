using System.Collections.Generic;
using UnityEngine;

namespace RagdollRealms.Core
{
    [DefaultExecutionOrder(-90)]
    public class PoolManager : MonoBehaviour, IPoolManager
    {
        private readonly Dictionary<int, Queue<Component>> _pools = new();
        private readonly Dictionary<int, int> _activeCount = new();
        private readonly Dictionary<int, Transform> _parents = new();

        private void Awake()
        {
            ServiceLocator.Instance.Register<IPoolManager>(this);
        }

        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            int key = prefab.GetInstanceID();

            if (_pools.TryGetValue(key, out var pool) && pool.Count > 0)
            {
                var instance = (T)pool.Dequeue();
                var go = instance.gameObject;
                go.transform.SetPositionAndRotation(position, rotation);
                go.SetActive(true);

                TrackActive(key, 1);

                if (instance is ISpawnable spawnable)
                    spawnable.Initialize();

                return instance;
            }

            var newInstance = Instantiate(prefab, position, rotation);
            EnsureParent(key, prefab.name);
            newInstance.transform.SetParent(_parents[key]);
            TrackActive(key, 1);
            return newInstance;
        }

        public void Despawn<T>(T instance) where T : Component
        {
            if (instance == null) return;

            if (instance is ISpawnable spawnable)
                spawnable.Reset();

            instance.gameObject.SetActive(false);

            int key = FindPrefabKey(instance);
            if (key == 0)
            {
                Debug.LogError($"[PoolManager] Despawn called on untracked object '{instance.gameObject.name}'. " +
                    "This object was not spawned through PoolManager. Ensure all spawning goes through Factories.");
                return;
            }

            if (!_pools.ContainsKey(key))
                _pools[key] = new Queue<Component>();

            _pools[key].Enqueue(instance);
            TrackActive(key, -1);
        }

        public void PreWarm<T>(T prefab, int count) where T : Component
        {
            int key = prefab.GetInstanceID();
            EnsureParent(key, prefab.name);

            if (!_pools.ContainsKey(key))
                _pools[key] = new Queue<Component>();

            for (int i = 0; i < count; i++)
            {
                var instance = Instantiate(prefab, _parents[key]);
                instance.gameObject.SetActive(false);
                _pools[key].Enqueue(instance);
            }
        }

        public int GetActiveCount<T>(T prefab) where T : Component
        {
            int key = prefab.GetInstanceID();
            return _activeCount.TryGetValue(key, out var count) ? count : 0;
        }

        public int GetPoolSize<T>(T prefab) where T : Component
        {
            int key = prefab.GetInstanceID();
            return _pools.TryGetValue(key, out var pool) ? pool.Count : 0;
        }

        private void EnsureParent(int key, string name)
        {
            if (_parents.ContainsKey(key)) return;
            var parent = new GameObject($"Pool_{name}");
            parent.transform.SetParent(transform);
            _parents[key] = parent.transform;
        }

        private void TrackActive(int key, int delta)
        {
            if (!_activeCount.ContainsKey(key))
                _activeCount[key] = 0;
            _activeCount[key] += delta;
        }

        private int FindPrefabKey(Component instance)
        {
            foreach (var kvp in _parents)
            {
                if (instance.transform.parent == kvp.Value)
                    return kvp.Key;
            }
            return 0;
        }

        private void OnDestroy()
        {
            if (ServiceLocator.Instance != null)
                ServiceLocator.Instance.Unregister<IPoolManager>();
        }
    }
}
