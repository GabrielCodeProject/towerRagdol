using UnityEngine;

namespace RagdollRealms.Systems.Ragdoll
{
    /// <summary>
    /// Lightweight reporter placed on each ragdoll bone.
    /// Forwards collision events to the central RagdollCollisionHandler.
    /// </summary>
    public class BoneCollisionReporter : MonoBehaviour
    {
        private RagdollCollisionHandler _handler;
        private int _jointIndex;
        private Transform _ragdollRoot;

        public void Initialize(RagdollCollisionHandler handler, int jointIndex, Transform ragdollRoot)
        {
            _handler = handler;
            _jointIndex = jointIndex;
            _ragdollRoot = ragdollRoot;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_handler == null) return;

            // Filter self-collisions (bones of the same ragdoll)
            if (collision.transform.root == _ragdollRoot) return;

            _handler.ReportBoneCollision(_jointIndex, collision);
        }
    }
}
