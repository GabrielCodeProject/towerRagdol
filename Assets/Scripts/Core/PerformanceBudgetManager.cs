using UnityEngine;

namespace RagdollRealms.Core
{
    public class PerformanceBudgetManager : MonoBehaviour, IPerformanceBudget
    {
        [Header("Budgets")]
        [SerializeField] private int _maxRagdolls = 32;
        [SerializeField] private int _maxEnemies = 40;

        private int _activeRagdolls;
        private int _activeEnemies;

        public int ActiveRagdolls => _activeRagdolls;
        public int ActiveEnemies => _activeEnemies;
        public int MaxRagdolls => _maxRagdolls;
        public int MaxEnemies => _maxEnemies;

        private void Awake()
        {
            ServiceLocator.Instance.Register<IPerformanceBudget>(this);
        }

        public bool CanSpawnRagdoll() => _activeRagdolls < _maxRagdolls;
        public bool CanSpawnEnemy() => _activeEnemies < _maxEnemies;

        public void TrackRagdoll(int delta)
        {
            _activeRagdolls = Mathf.Max(0, _activeRagdolls + delta);
        }

        public void TrackEnemy(int delta)
        {
            _activeEnemies = Mathf.Max(0, _activeEnemies + delta);
        }

        private void OnDestroy()
        {
            if (ServiceLocator.Instance != null)
                ServiceLocator.Instance.Unregister<IPerformanceBudget>();
        }
    }
}
