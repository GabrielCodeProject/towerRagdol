using UnityEngine;

namespace RagdollRealms.Core
{
    public interface IBossPhase
    {
        void Enter(GameObject boss);
        void Update(GameObject boss);
        void Exit(GameObject boss);
        bool IsComplete { get; }
    }
}
