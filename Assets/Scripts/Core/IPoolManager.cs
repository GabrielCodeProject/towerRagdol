using UnityEngine;

namespace RagdollRealms.Core
{
    public interface IPoolManager
    {
        T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component;
        void Despawn<T>(T instance) where T : Component;
        void PreWarm<T>(T prefab, int count) where T : Component;
        int GetActiveCount<T>(T prefab) where T : Component;
        int GetPoolSize<T>(T prefab) where T : Component;
    }
}
