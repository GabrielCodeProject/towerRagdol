using UnityEngine;
using UnityEngine.InputSystem;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;

namespace RagdollRealms.Systems.Player
{
    public class PlayerInputController : MonoBehaviour
    {
        [Header("Input Action References")]
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _jumpAction;
        [SerializeField] private InputActionReference _sprintAction;
        [SerializeField] private InputActionReference _lookAction;
        [SerializeField] private int _playerId;

        private IEventBus _eventBus;

        public Vector2 CurrentMoveInput { get; private set; }
        public Vector2 CurrentLookInput { get; private set; }
        public bool SprintHeld { get; private set; }

        private void Start()
        {
            ServiceLocator.Instance.TryGet(out _eventBus);
        }

        private void OnEnable()
        {
            if (_moveAction != null) _moveAction.action.Enable();
            if (_lookAction != null) _lookAction.action.Enable();
            if (_jumpAction != null)
            {
                _jumpAction.action.Enable();
                _jumpAction.action.performed += OnJumpPerformed;
            }
            if (_sprintAction != null)
            {
                _sprintAction.action.Enable();
                _sprintAction.action.performed += OnSprintPerformed;
                _sprintAction.action.canceled += OnSprintCanceled;
            }
        }

        private void OnDisable()
        {
            if (_moveAction != null) _moveAction.action.Disable();
            if (_lookAction != null) _lookAction.action.Disable();
            if (_jumpAction != null)
            {
                _jumpAction.action.performed -= OnJumpPerformed;
                _jumpAction.action.Disable();
            }
            if (_sprintAction != null)
            {
                _sprintAction.action.performed -= OnSprintPerformed;
                _sprintAction.action.canceled -= OnSprintCanceled;
                _sprintAction.action.Disable();
            }
        }

        private void Update()
        {
            if (_moveAction != null)
                CurrentMoveInput = _moveAction.action.ReadValue<Vector2>();

            if (_lookAction != null)
                CurrentLookInput = _lookAction.action.ReadValue<Vector2>();
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            _eventBus?.Publish(new OnJumpRequested(_playerId));
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            SprintHeld = true;
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            SprintHeld = false;
        }
    }
}
