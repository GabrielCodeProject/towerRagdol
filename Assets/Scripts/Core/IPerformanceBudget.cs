namespace RagdollRealms.Core
{
    public interface IPerformanceBudget
    {
        bool CanSpawnRagdoll();
        bool CanSpawnEnemy();
        void TrackRagdoll(int delta);
        void TrackEnemy(int delta);
        int ActiveRagdolls { get; }
        int ActiveEnemies { get; }
        int MaxRagdolls { get; }
        int MaxEnemies { get; }
    }
}
