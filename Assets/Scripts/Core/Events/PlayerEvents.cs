using UnityEngine;

namespace RagdollRealms.Core.Events
{
    public readonly struct OnJumpRequested
    {
        public readonly int PlayerId;

        public OnJumpRequested(int playerId)
        {
            PlayerId = playerId;
        }
    }

    public readonly struct OnPlayerJumped
    {
        public readonly int PlayerId;
        public readonly Vector3 Position;

        public OnPlayerJumped(int playerId, Vector3 position)
        {
            PlayerId = playerId;
            Position = position;
        }
    }

    public readonly struct OnPlayerLanded
    {
        public readonly int PlayerId;
        public readonly Vector3 Position;

        public OnPlayerLanded(int playerId, Vector3 position)
        {
            PlayerId = playerId;
            Position = position;
        }
    }

    public readonly struct OnPlayerSprintChanged
    {
        public readonly int PlayerId;
        public readonly bool IsSprinting;

        public OnPlayerSprintChanged(int playerId, bool isSprinting)
        {
            PlayerId = playerId;
            IsSprinting = isSprinting;
        }
    }

    public readonly struct OnPlayerSpeedChanged
    {
        public readonly int PlayerId;
        public readonly float Speed;

        public OnPlayerSpeedChanged(int playerId, float speed)
        {
            PlayerId = playerId;
            Speed = speed;
        }
    }
}
