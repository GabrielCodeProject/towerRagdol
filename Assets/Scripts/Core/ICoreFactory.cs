using UnityEngine;

namespace RagdollRealms.Core
{
    public interface ICoreFactory
    {
        GameObject SpawnCore(Vector3 position);
    }
}
