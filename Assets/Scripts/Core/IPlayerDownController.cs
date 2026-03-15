using System.Collections.Generic;

namespace RagdollRealms.Core
{
    public interface IPlayerDownController
    {
        bool IsPlayerDowned(int playerId);
        IReadOnlyCollection<int> GetDownedPlayers();
        void StartRevive(int reviverId, int downedPlayerId);
        void CancelRevive(int downedPlayerId);
        bool IsReviveInProgress(int downedPlayerId);
    }
}
