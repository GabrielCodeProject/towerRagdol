using System;
using UnityEngine;
using RagdollRealms.Content;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;

namespace RagdollRealms.Systems.Player
{
    public class PlayerMovementController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private PlayerConfigDefinition _config;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private int _playerId;

        private PlayerInputController _inputController;
        private IEventBus _eventBus;
        private Rigidbody _hipRigidbody;
        private bool _movementEnabled = true;
        private bool _wasGrounded;
        private bool _initialized;
        private float _lastPublishedSpeed = -1f;

        private Action<OnJumpRequested> _onJumpRequested;

        public Vector3 MovementDirection { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool IsGrounded { get; private set; }
        public Rigidbody HipRigidbody => _hipRigidbody;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_initialized) return;
            if (ServiceLocator.Instance == null) return;

            _inputController = GetComponent<PlayerInputController>();

            ServiceLocator.Instance.TryGet(out _eventBus);
            ServiceLocator.Instance.Register<IPlayerController>(this);

            var ragdoll = GetComponent<IRagdollController>();
            if (ragdoll != null)
            {
                _hipRigidbody = ragdoll.HipRigidbody;
            }

            if (_cameraTransform == null)
            {
                var mainCam = Camera.main;
                if (mainCam != null) _cameraTransform = mainCam.transform;
            }

            if (_eventBus != null)
            {
                _onJumpRequested = HandleJumpRequested;
                _eventBus.Subscribe(_onJumpRequested);
            }

            _initialized = _hipRigidbody != null;
        }

        private void FixedUpdate()
        {
            if (!_initialized) { Initialize(); return; }
            if (!_movementEnabled) return;

            UpdateGroundCheck();
            CheckLanding();
            UpdateMovement();
            UpdateSprintState();
        }

        private void UpdateGroundCheck()
        {
            _wasGrounded = IsGrounded;
            IsGrounded = Physics.Raycast(
                _hipRigidbody.position,
                Vector3.down,
                _config.GroundCheckDistance,
                _config.GroundLayerMask
            );
        }

        private void CheckLanding()
        {
            if (!_wasGrounded && IsGrounded)
            {
                _eventBus?.Publish(new OnPlayerLanded(_playerId, _hipRigidbody.position));
            }
        }

        private void UpdateMovement()
        {
            Vector2 input = _inputController != null ? _inputController.CurrentMoveInput : Vector2.zero;
            float inputMagnitude = input.magnitude;

            // Publish speed for AnimationFollower via EventBus: 0=idle, 0.5=walk, 1=run
            float targetSpeed = IsSprinting ? inputMagnitude : inputMagnitude * 0.5f;
            PublishSpeedIfChanged(targetSpeed);

            if (inputMagnitude < 0.01f)
            {
                MovementDirection = Vector3.zero;
                return;
            }

            // Calculate camera-relative direction
            Vector3 cameraForward = Vector3.forward;
            Vector3 cameraRight = Vector3.right;

            if (_cameraTransform != null)
            {
                cameraForward = _cameraTransform.forward;
                cameraRight = _cameraTransform.right;
            }

            cameraForward.y = 0f;
            cameraForward.Normalize();
            cameraRight.y = 0f;
            cameraRight.Normalize();

            MovementDirection = (cameraForward * input.y + cameraRight * input.x).normalized;

            // Rotate root to face movement direction
            if (MovementDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(MovementDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    _config.RotationSpeed * Time.fixedDeltaTime
                );
            }

            // Move via physics to avoid joint stretching
            float speed = _config.MoveSpeed * (IsSprinting ? _config.SprintMultiplier : 1f);
            Vector3 targetPosition = _hipRigidbody.position + MovementDirection * speed * Time.fixedDeltaTime;
            _hipRigidbody.MovePosition(targetPosition);
        }

        private void PublishSpeedIfChanged(float speed)
        {
            if (Mathf.Approximately(_lastPublishedSpeed, speed)) return;
            _lastPublishedSpeed = speed;
            _eventBus?.Publish(new OnPlayerSpeedChanged(_playerId, speed));
        }

        private void UpdateSprintState()
        {
            bool wasSprinting = IsSprinting;
            IsSprinting = _inputController != null && _inputController.SprintHeld;

            if (wasSprinting != IsSprinting)
            {
                _eventBus?.Publish(new OnPlayerSprintChanged(_playerId, IsSprinting));
            }
        }

        private void HandleJumpRequested(OnJumpRequested evt)
        {
            if (evt.PlayerId != _playerId) return;
            TryJump();
        }

        private void TryJump()
        {
            if (!_movementEnabled || !IsGrounded || _hipRigidbody == null) return;

            _hipRigidbody.AddForce(Vector3.up * _config.JumpForce, ForceMode.Impulse);
            _eventBus?.Publish(new OnPlayerJumped(_playerId, _hipRigidbody.position));
        }

        public void SetMovementEnabled(bool enabled)
        {
            _movementEnabled = enabled;
            if (!enabled)
            {
                PublishSpeedIfChanged(0f);
            }
        }

        private void OnDestroy()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe(_onJumpRequested);
            }

            if (ServiceLocator.Instance != null)
                ServiceLocator.Instance.Unregister<IPlayerController>();
        }
    }
}
