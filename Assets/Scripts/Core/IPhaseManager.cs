namespace RagdollRealms.Core
{
    public interface IPhaseManager
    {
        PhaseType CurrentPhase { get; }
        float PrepareTimeRemaining { get; }
        int CurrentWaveNumber { get; }
        void StartGame();
        void PlayerReady(int playerId);
        bool IsInPhase(PhaseType phase);
    }
}
