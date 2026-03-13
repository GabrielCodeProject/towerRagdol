using UnityEngine;

namespace RagdollRealms.Core
{
    public interface IEnemyBehavior
    {
        void Think(GameObject enemy, Vector3 targetPosition);
        void Act(GameObject enemy);
        void OnHit(GameObject enemy, float damage, Vector3 hitDirection);
        void OnDeath(GameObject enemy);
    }
}
