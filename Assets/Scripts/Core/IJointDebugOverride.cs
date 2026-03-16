namespace RagdollRealms.Core
{
    /// <summary>
    /// Interface for debug tools that can lock individual joint springs
    /// and disable the procedural walk system. Implemented by RagdollJointDebugger.
    /// </summary>
    public interface IJointDebugOverride
    {
        bool IsJointLocked(int jointIndex);
        bool IsWalkSystemDisabled { get; }
    }
}
