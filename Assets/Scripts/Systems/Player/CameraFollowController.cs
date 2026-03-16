using UnityEngine;
using RagdollRealms.Content;
using RagdollRealms.Core;

namespace RagdollRealms.Systems.Player
{
    public class CameraFollowController : MonoBehaviour
    {
        [SerializeField] private PlayerConfigDefinition _config;
        [SerializeField] private Transform _target;

        private Rigidbody _hipRigidbody;

        private void Start()
        {
            // Follow the hip rigidbody (where the ragdoll actually is)
            // instead of the root transform (which can drift from the ragdoll).
            if (_target != null)
            {
                var ragdoll = _target.GetComponent<IRagdollController>();
                if (ragdoll != null)
                    _hipRigidbody = ragdoll.HipRigidbody;
            }
        }

        private void LateUpdate()
        {
            if (_target == null || _config == null) return;

            // Use hip position for camera tracking, root rotation for look direction
            Vector3 followPos = _hipRigidbody != null ? _hipRigidbody.position : _target.position;

            Vector3 targetPosition = followPos + _config.CameraOffset;
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                _config.CameraFollowSpeed * Time.deltaTime
            );

            Vector3 lookTarget = followPos + _target.forward * _config.CameraLookAhead;
            transform.LookAt(lookTarget);
        }
    }
}
