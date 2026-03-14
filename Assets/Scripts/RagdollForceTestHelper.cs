using UnityEngine;
using UnityEngine.InputSystem;
using RagdollRealms.Core;
using RagdollRealms.Systems.Ragdoll;

/// <summary>
/// Temporary test helper — attach to a ragdoll to test forces during Play mode.
/// SPACE = impulse push, F = explosion, R = reset position, I = info.
/// Lives outside assembly definitions so it can reference both InputSystem and game assemblies.
/// </summary>
public class RagdollForceTestHelper : MonoBehaviour
{
    [Header("Force Settings")]
    [SerializeField] private float _pushForce = 80f;
    [SerializeField] private float _explosionForce = 120f;
    [SerializeField] private Vector3 _pushDirection = Vector3.back;

    private Vector3 _startPosition;
    private Quaternion _startRotation;

    private RagdollForceReceiver _forceReceiver;
    private IRagdollController _controller;
    private RagdollRecoveryController _recovery;
    private bool _initialized;

    private void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (!_initialized)
        {
            _forceReceiver = GetComponent<RagdollForceReceiver>();
            _controller = GetComponent<IRagdollController>();
            _recovery = GetComponent<RagdollRecoveryController>();
            if (_forceReceiver != null && _controller != null)
                _initialized = true;
            return;
        }

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            var force = _pushDirection.normalized * _pushForce;
            _forceReceiver.ApplyForce(force, ForceType.Impact);
            Debug.Log($"[Test] Applied impulse: {force} (magnitude: {force.magnitude:F1})");
        }

        if (keyboard.fKey.wasPressedThisFrame)
        {
            var force = Vector3.up * _explosionForce;
            _forceReceiver.ApplyForce(force, ForceType.Explosion);
            Debug.Log($"[Test] Applied explosion: {force.magnitude:F1}");
        }

        if (keyboard.rKey.wasPressedThisFrame)
        {
            ResetRagdoll();
            Debug.Log("[Test] Ragdoll reset to start position");
        }

        if (keyboard.iKey.wasPressedThisFrame)
        {
            bool recovering = _recovery != null && _recovery.IsRecovering;
            Debug.Log($"[Test] IsRagdolling={_controller.IsRagdolling}, " +
                      $"Bodies={_controller.AllBodies.Count}, " +
                      $"Joints={_controller.AllJoints.Count}, " +
                      $"IsRecovering={recovering}");
        }
    }

    private void ResetRagdoll()
    {
        if (_controller != null)
        {
            foreach (var body in _controller.AllBodies)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }
        }

        _controller?.SetKinematic(true);

        transform.position = _startPosition;
        transform.rotation = _startRotation;

        if (_controller is ISpawnable spawnable)
            spawnable.Initialize();
    }

    private void OnGUI()
    {
        if (!_initialized || _controller == null) return;

        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            normal = { textColor = Color.white }
        };

        float y = 10;
        GUI.Label(new Rect(10, y, 400, 25), "=== Ragdoll Test Controls ===", style);
        y += 20;
        GUI.Label(new Rect(10, y, 400, 25), "SPACE = Push  |  F = Explosion  |  R = Reset  |  I = Info", style);
        y += 20;

        string state = _controller.IsRagdolling ? "RAGDOLLING" : "ACTIVE";
        bool recovering = _recovery != null && _recovery.IsRecovering;
        string recStr = recovering ? " (RECOVERING)" : "";
        GUI.Label(new Rect(10, y, 400, 25), $"State: {state}{recStr}", style);
    }
}
