using System.Collections.Generic;
using UnityEngine;

namespace RagdollRealms.Core
{
    public interface ITargetingStrategy
    {
        Transform SelectTarget(Vector3 origin, IReadOnlyList<Transform> candidates);
    }
}
