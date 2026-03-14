using UnityEngine;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewPlayerConfig", menuName = "Ragdoll Realms/Content/Player Config")]
    public class PlayerConfigDefinition : ContentDefinition
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 8f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _sprintMultiplier = 1.6f;
        [SerializeField] private float _jumpForce = 7f;

        [Header("Ground Detection")]
        [SerializeField] private float _groundCheckDistance = 1.8f;
        [SerializeField] private LayerMask _groundLayerMask;

        [Header("Camera")]
        [SerializeField] private float _cameraFollowSpeed = 8f;
        [SerializeField] private Vector3 _cameraOffset = new(0f, 5f, -7f);
        [SerializeField] private float _cameraLookAhead = 2f;

        public float MoveSpeed => _moveSpeed;
        public float RotationSpeed => _rotationSpeed;
        public float SprintMultiplier => _sprintMultiplier;
        public float JumpForce => _jumpForce;
        public float GroundCheckDistance => _groundCheckDistance;
        public LayerMask GroundLayerMask => _groundLayerMask;
        public float CameraFollowSpeed => _cameraFollowSpeed;
        public Vector3 CameraOffset => _cameraOffset;
        public float CameraLookAhead => _cameraLookAhead;
    }
}
