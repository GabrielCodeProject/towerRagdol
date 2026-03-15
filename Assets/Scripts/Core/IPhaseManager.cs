namespace RagdollRealms.Core
{
    public interface IPhaseManager
    {
        PhaseType CurrentPhase { get; }
        float PrepareTimeRemaining { get; }
        int CurrentWaveNumber { get; }
        int EnemiesKilled { get; }
        int BossesDefeated { get; }
        int StructuresDamaged { get; }
        float GameStartTime { get; }
        void StartGame();
        void PlayerReady(int playerId);
        bool IsInPhase(PhaseType phase);
    }
}
