using UnityEngine;

namespace RagdollRealms.Core
{
    public interface IPlayerController
    {
        Vector3 MovementDirection { get; }
        bool IsSprinting { get; }
        bool IsGrounded { get; }
        Rigidbody HipRigidbody { get; }
        void SetMovementEnabled(bool enabled);
    }
}
