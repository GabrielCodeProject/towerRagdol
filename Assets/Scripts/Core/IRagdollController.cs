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
        void SetJointSpringMultiplier(float multiplier);
        void SetKinematic(bool kinematic);
    }
}
