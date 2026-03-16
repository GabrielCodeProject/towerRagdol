using UnityEngine;
using UnityEngine.InputSystem;
using RagdollRealms.Core;
using RagdollRealms.Systems.Ragdoll;
using RagdollRealms.Systems.Player;

namespace RagdollRealms.Debugging
{
    /// <summary>
    /// Gizmo debugger for ragdoll center of gravity, rotation pivot, and movement pivot.
    /// Visualizes why the ragdoll orbits a forward point during rapid WASD input.
    ///
    /// Draws:
    ///   Green sphere  = Root transform position (movement application point)
    ///   Red sphere    = Computed center of mass (weighted average of all rigidbodies)
    ///   Cyan sphere   = Hip rigidbody position (physics anchor)
    ///   Yellow line   = Root → COM offset (drift indicator)
    ///   Magenta line  = Root → Hip offset
    ///   White arrow   = Root forward direction
    ///   Orange arrow  = Hip velocity direction
    ///
    /// Toggle: RShift + B
    /// </summary>
    public class RagdollGravityGizmo : MonoBehaviour
    {
        [Header("Gizmo Sizes")]
        [SerializeField] private float _sphereRadius = 0.12f;
        [SerializeField] private float _arrowLength = 1.5f;

        private IRagdollController _controller;
        private PlayerMovementController _movementController;
        private RagdollBalanceForce _balanceForce;
        private Rigidbody _hipRb;
        private bool _initialized;
        private bool _visible = true;

        // Cached values for OnDrawGizmos (which runs outside Update)
        private Vector3 _cachedCom;
        private Vector3 _cachedRootPos;
        private Vector3 _cachedHipPos;
        private Vector3 _cachedRootForward;
        private Vector3 _cachedHipVelocity;
        private Vector3 _cachedMovementDir;
        private float _cachedComToRootDist;
        private float _cachedHipToRootDist;
        private float _cachedTotalMass;

        private void Start()
        {
            TryInitialize();
        }

        private void TryInitialize()
        {
            _controller = GetComponent<IRagdollController>();
            _movementController = GetComponent<PlayerMovementController>();
            _balanceForce = GetComponent<RagdollBalanceForce>();

            if (_controller == null) return;

            _hipRb = _controller.HipRigidbody;
            if (_hipRb == null) return;

            _cachedTotalMass = 0f;
            foreach (var body in _controller.AllBodies)
                _cachedTotalMass += body.mass;

            _initialized = true;
            Debug.Log("[GravityGizmo] Initialized — RShift+B to toggle");
        }

        private void Update()
        {
            if (!_initialized) { TryInitialize(); return; }

            var kb = Keyboard.current;
            if (kb != null && kb.rightShiftKey.isPressed && kb.bKey.wasPressedThisFrame)
            {
                _visible = !_visible;
                Debug.Log($"[GravityGizmo] Visibility: {(_visible ? "ON" : "OFF")}");
            }

            if (!_visible) return;

            CacheValues();
        }

        private void CacheValues()
        {
            _cachedRootPos = transform.position;
            _cachedHipPos = _hipRb.position;
            _cachedRootForward = transform.forward;
            _cachedHipVelocity = _hipRb.linearVelocity;
            _cachedMovementDir = _movementController != null
                ? _movementController.MovementDirection
                : Vector3.zero;

            // Compute weighted center of mass across all ragdoll bodies
            Vector3 weightedSum = Vector3.zero;
            foreach (var body in _controller.AllBodies)
            {
                weightedSum += body.worldCenterOfMass * body.mass;
            }
            _cachedCom = _cachedTotalMass > 0f ? weightedSum / _cachedTotalMass : _cachedHipPos;

            _cachedComToRootDist = Vector3.Distance(_cachedCom, _cachedRootPos);
            _cachedHipToRootDist = Vector3.Distance(_cachedHipPos, _cachedRootPos);
        }

        private void OnDrawGizmos()
        {
            if (!_initialized || !_visible) return;

            // Root transform — movement application point
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_cachedRootPos, _sphereRadius);

            // Center of mass — weighted average
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_cachedCom, _sphereRadius * 1.3f);

            // Hip rigidbody position
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(_cachedHipPos, _sphereRadius);

            // Root → COM offset (drift indicator)
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_cachedRootPos, _cachedCom);
            Gizmos.DrawWireSphere(_cachedCom, _cachedComToRootDist > 0.05f ? 0.08f : 0.03f);

            // Root → Hip offset
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(_cachedRootPos, _cachedHipPos);

            // Root forward direction
            Gizmos.color = Color.white;
            DrawArrow(_cachedRootPos, _cachedRootForward, _arrowLength);

            // Hip velocity direction
            if (_cachedHipVelocity.sqrMagnitude > 0.01f)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f); // orange
                DrawArrow(_cachedHipPos, _cachedHipVelocity.normalized,
                    Mathf.Min(_cachedHipVelocity.magnitude * 0.3f, _arrowLength));
            }

            // Movement direction (input-derived)
            if (_cachedMovementDir.sqrMagnitude > 0.01f)
            {
                Gizmos.color = new Color(0.5f, 1f, 0.5f); // light green
                DrawArrow(_cachedRootPos + Vector3.up * 0.1f, _cachedMovementDir, _arrowLength * 0.8f);
            }

            // Ground projection lines (vertical drop from each point)
            Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
            Vector3 groundRoot = new Vector3(_cachedRootPos.x, _cachedRootPos.y - 1f, _cachedRootPos.z);
            Vector3 groundCom = new Vector3(_cachedCom.x, _cachedRootPos.y - 1f, _cachedCom.z);
            Vector3 groundHip = new Vector3(_cachedHipPos.x, _cachedRootPos.y - 1f, _cachedHipPos.z);
            Gizmos.DrawLine(_cachedRootPos, groundRoot);
            Gizmos.DrawLine(_cachedCom, groundCom);
            Gizmos.DrawLine(_cachedHipPos, groundHip);

            // Ground-plane triangle showing the three points
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(groundRoot, groundCom);
            Gizmos.DrawLine(groundRoot, groundHip);
            Gizmos.DrawLine(groundCom, groundHip);
        }

        private static void DrawArrow(Vector3 origin, Vector3 direction, float length)
        {
            if (direction.sqrMagnitude < 0.001f) return;

            Vector3 tip = origin + direction.normalized * length;
            Gizmos.DrawLine(origin, tip);

            // Arrowhead
            Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
            if (right.sqrMagnitude < 0.001f)
                right = Vector3.Cross(direction, Vector3.forward).normalized;

            float headSize = length * 0.15f;
            Vector3 back = -direction.normalized * headSize;
            Gizmos.DrawLine(tip, tip + back + right * headSize * 0.5f);
            Gizmos.DrawLine(tip, tip + back - right * headSize * 0.5f);
        }

        private void OnGUI()
        {
            if (!_initialized || !_visible) return;

            var headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.cyan }
            };
            var normalStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = Color.white }
            };
            var warnStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.yellow }
            };

            float panelWidth = 420f;
            float x = Screen.width - panelWidth - 10f;
            float y = 300f; // Below collision wobble panel

            GUI.Label(new Rect(x, y, panelWidth, 20), "=== Gravity / Pivot Debug ===", headerStyle);
            y += 18;
            GUI.Label(new Rect(x, y, panelWidth, 20), "RShift+B = Toggle", normalStyle);
            y += 22;

            // Positions
            GUI.Label(new Rect(x, y, panelWidth, 20),
                $"Root (green):  {FormatV3(_cachedRootPos)}", normalStyle);
            y += 16;
            GUI.Label(new Rect(x, y, panelWidth, 20),
                $"Hip  (cyan):   {FormatV3(_cachedHipPos)}", normalStyle);
            y += 16;
            GUI.Label(new Rect(x, y, panelWidth, 20),
                $"COM  (red):    {FormatV3(_cachedCom)}", normalStyle);
            y += 22;

            // Offsets — this is the key diagnostic
            bool drifting = _cachedComToRootDist > 0.15f;
            var offsetStyle = drifting ? warnStyle : normalStyle;
            GUI.Label(new Rect(x, y, panelWidth, 20),
                $"Root→COM offset: {_cachedComToRootDist:F3}m {(drifting ? "⚠ DRIFTING" : "OK")}",
                offsetStyle);
            y += 16;

            Vector3 comOffset = _cachedCom - _cachedRootPos;
            GUI.Label(new Rect(x, y, panelWidth, 20),
                $"  X:{comOffset.x:+0.00;-0.00}  Y:{comOffset.y:+0.00;-0.00}  Z:{comOffset.z:+0.00;-0.00}",
                normalStyle);
            y += 16;

            bool hipDrifting = _cachedHipToRootDist > 0.2f;
            var hipStyle = hipDrifting ? warnStyle : normalStyle;
            GUI.Label(new Rect(x, y, panelWidth, 20),
                $"Root→Hip offset: {_cachedHipToRootDist:F3}m {(hipDrifting ? "⚠ DRIFTING" : "OK")}",
                hipStyle);
            y += 22;

            // Velocity
            GUI.Label(new Rect(x, y, panelWidth, 20),
                $"Hip velocity: {_cachedHipVelocity.magnitude:F2} m/s  dir: {FormatV3(_cachedHipVelocity.normalized)}",
                normalStyle);
            y += 16;

            // Forward vs movement angle
            if (_cachedMovementDir.sqrMagnitude > 0.01f)
            {
                float angle = Vector3.SignedAngle(_cachedRootForward, _cachedMovementDir, Vector3.up);
                GUI.Label(new Rect(x, y, panelWidth, 20),
                    $"Forward↔Move angle: {angle:+0.0;-0.0}°", normalStyle);
                y += 16;
            }

            // Dot product: is COM ahead or behind root?
            if (comOffset.sqrMagnitude > 0.001f)
            {
                float forwardDot = Vector3.Dot(comOffset.normalized, _cachedRootForward);
                string relation = forwardDot > 0.1f ? "COM is AHEAD of root"
                    : forwardDot < -0.1f ? "COM is BEHIND root"
                    : "COM is lateral to root";
                GUI.Label(new Rect(x, y, panelWidth, 20),
                    $"COM position: {relation} (dot: {forwardDot:F2})", normalStyle);
                y += 16;
            }

            GUI.Label(new Rect(x, y, panelWidth, 20),
                $"Total mass: {_cachedTotalMass:F1} kg", normalStyle);
        }

        private static string FormatV3(Vector3 v)
        {
            return $"({v.x:+0.00;-0.00}, {v.y:+0.00;-0.00}, {v.z:+0.00;-0.00})";
        }
    }
}
