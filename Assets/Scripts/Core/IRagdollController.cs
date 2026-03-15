using System.Collections.Generic;
using UnityEngine;

namespace RagdollRealms.Core
{
    public interface IRagdollController
    {
        Rigidbody HipRigidbody { get; }
        IReadOnlyList<ConfigurableJoint> AllJoints { get; }
        IReadOnlyList<Rigidbody> AllBodies { get; }
        bool IsRagdolling { get; set; }
        IRagdollConfig Config { get; }
        int JointCount { get; }
        void SetJointSpringMultiplier(float multiplier);
        void SetJointSpringMultiplier(int jointIndex, float multiplier);
        float GetJointSpringMultiplier(int jointIndex);
        int GetJointIndex(ConfigurableJoint joint);
        void SetKinematic(bool kinematic);
    }
}
