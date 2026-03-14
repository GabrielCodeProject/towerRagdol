using UnityEngine;
using RagdollRealms.Content;

namespace RagdollRealms.Systems.Player
{
    public class CameraFollowController : MonoBehaviour
    {
        [SerializeField] private PlayerConfigDefinition _config;
        [SerializeField] private Transform _target;

        private void LateUpdate()
        {
            if (_target == null || _config == null) return;

            Vector3 targetPosition = _target.position + _config.CameraOffset;
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                _config.CameraFollowSpeed * Time.deltaTime
            );

            Vector3 lookTarget = _target.position + _target.forward * _config.CameraLookAhead;
            transform.LookAt(lookTarget);
        }
    }
}
