using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using RagdollRealms.Core;
using RagdollRealms.Systems.Player;

namespace RagdollRealms.Debugging
{
    /// <summary>
    /// Interactive runtime joint inspector for the ragdoll.
    /// Click bones to select, drag to apply forces, OnGUI panel for properties.
    /// Toggle: RShift + J
    ///
    /// Controls:
    ///   Click bone      = Select joint
    ///   Drag            = Apply force toward mouse
    ///   LShift + Drag   = Apply torque
    ///   OnGUI sliders   = Adjust spring, apply impulses
    /// </summary>
    public class RagdollJointDebugger : MonoBehaviour, IJointDebugOverride
    {
        [Header("Settings")]
        [SerializeField] private float _dragForce = 300f;
        [SerializeField] private float _torqueForce = 50f;
        [SerializeField] private float _impulseStrength = 5f;

        [Header("Debug Camera")]
        [SerializeField] private float _orbitSpeed = 3f;
        [SerializeField] private float _zoomSpeed = 2f;
        [SerializeField] private float _panSpeed = 0.5f;
        [SerializeField] private float _minZoomDist = 0.5f;
        [SerializeField] private float _maxZoomDist = 20f;

        private IRagdollController _controller;
        private bool _enabled;
        private bool _initialized;

        // Selection
        private int _selectedIndex = -1;
        private Rigidbody _selectedRb;
        private ConfigurableJoint _selectedJoint;
        private string _selectedName = "";
        private float _selectionDistance;
        private bool _isDragging;

        // Cached drag state
        private Vector3 _dragTarget;

        // Rigidbody → joint index lookup
        private Dictionary<Rigidbody, int> _rbToJointIndex;

        // Per-joint spring overrides for slider
        private float[] _springOverrides;
        private bool[] _jointLocked; // debug-locked joints ignore external spring changes
        private bool _walkSystemDisabled;
        private bool _pinHipWhileDragging = true;
        private bool _hipFrozen;

        // Debug camera
        private Camera _cam;
        private MonoBehaviour _cameraFollower; // CameraFollowController to disable when debugging
        private float _camDistance = 5f;
        private float _camYaw;
        private float _camPitch = 20f;
        private Vector3 _camPivotOffset; // pan offset from hip

        // GUI state
        private Vector2 _scrollPos;
        private GUIStyle _headerStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _valueStyle;

        private void Start()
        {
            _controller = GetComponent<IRagdollController>();
            if (_controller == null) return;

            _rbToJointIndex = new Dictionary<Rigidbody, int>();
            for (int i = 0; i < _controller.AllJoints.Count; i++)
            {
                var rb = _controller.AllJoints[i].GetComponent<Rigidbody>();
                if (rb != null)
                    _rbToJointIndex[rb] = i;
            }

            _springOverrides = new float[_controller.JointCount];
            _jointLocked = new bool[_controller.JointCount];
            for (int i = 0; i < _springOverrides.Length; i++)
                _springOverrides[i] = 1f;

            _cam = Camera.main;
            if (_cam == null)
                _cam = FindAnyObjectByType<Camera>();
            if (_cam != null)
                _cameraFollower = _cam.GetComponent<CameraFollowController>();

            // Initialize orbit from current camera position
            if (_cam != null && _controller?.HipRigidbody != null)
            {
                Vector3 toCamera = _cam.transform.position - _controller.HipRigidbody.position;
                _camDistance = toCamera.magnitude;
                _camYaw = Mathf.Atan2(toCamera.x, toCamera.z) * Mathf.Rad2Deg;
                _camPitch = Mathf.Asin(toCamera.y / _camDistance) * Mathf.Rad2Deg;
            }

            _initialized = true;
        }

        private void Update()
        {
            if (!_initialized) return;

            var kb = Keyboard.current;
            if (kb == null) return;

            // Toggle: RShift + J
            if (kb.rightShiftKey.isPressed && kb.jKey.wasPressedThisFrame)
            {
                _enabled = !_enabled;
                if (!_enabled)
                {
                    _selectedIndex = -1;
                    _selectedRb = null;
                    _isDragging = false;

                    // Restore hip physics when debugger is turned off
                    if (_hipFrozen && _controller?.HipRigidbody != null)
                    {
                        _controller.HipRigidbody.isKinematic = false;
                        _controller.HipRigidbody.linearVelocity = Vector3.zero;
                        _controller.HipRigidbody.angularVelocity = Vector3.zero;
                        _hipFrozen = false;
                    }
                }

                // Disable camera follow controller so it doesn't fight the debug camera
                if (_cameraFollower != null)
                    _cameraFollower.enabled = !_enabled;

                Debug.Log($"[JointDebugger] {(_enabled ? "ON — camera control active" : "OFF — camera returned")}");
            }

            if (!_enabled) return;

            var mouse = Mouse.current;
            if (mouse == null) return;

            // Left click: select bone
            if (mouse.leftButton.wasPressedThisFrame)
            {
                TrySelectBone(mouse.position.ReadValue());
            }

            // Drag tracking (left click on bone)
            if (mouse.leftButton.isPressed && _selectedRb != null)
            {
                _isDragging = true;
                UpdateDragTarget(mouse.position.ReadValue());
            }
            else
            {
                _isDragging = false;
            }

            // Camera: right-click drag to orbit
            if (mouse.rightButton.isPressed)
            {
                Vector2 delta = mouse.delta.ReadValue();
                _camYaw += delta.x * _orbitSpeed * 0.1f;
                _camPitch -= delta.y * _orbitSpeed * 0.1f;
                _camPitch = Mathf.Clamp(_camPitch, -80f, 80f);
            }

            // Camera: middle-click drag to pan
            if (mouse.middleButton.isPressed)
            {
                Vector2 delta = mouse.delta.ReadValue();
                if (_cam != null)
                {
                    _camPivotOffset -= _cam.transform.right * delta.x * _panSpeed * 0.01f;
                    _camPivotOffset -= _cam.transform.up * delta.y * _panSpeed * 0.01f;
                }
            }

            // Camera: Q/E to zoom
            float zoomInput = 0f;
            if (kb.qKey.isPressed) zoomInput = 1f;
            if (kb.eKey.isPressed) zoomInput = -1f;

            float scrollRaw = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scrollRaw) > 0.1f)
                zoomInput += Mathf.Sign(scrollRaw);

            if (Mathf.Abs(zoomInput) > 0.01f)
            {
                // Move camera closer/further
                float zoomStep = _camDistance * _zoomSpeed * Time.deltaTime * zoomInput;
                _camDistance -= zoomStep;
                _camDistance = Mathf.Clamp(_camDistance, _minZoomDist, _maxZoomDist);

                // Also adjust FOV/ortho size so zoom actually changes what you see
                if (_cam != null)
                {
                    if (_cam.orthographic)
                    {
                        _cam.orthographicSize -= zoomInput * _zoomSpeed * Time.deltaTime;
                        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, 0.5f, 20f);
                    }
                    else
                    {
                        _cam.fieldOfView -= zoomInput * _zoomSpeed * 10f * Time.deltaTime;
                        _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView, 10f, 90f);
                    }
                }
            }
        }

        private void LateUpdate()
        {
            if (!_enabled || !_initialized || _cam == null || _controller == null) return;

            // Orbit camera around hip (or selected bone)
            Vector3 pivot = _controller.HipRigidbody != null
                ? _controller.HipRigidbody.position
                : transform.position;
            pivot += _camPivotOffset;

            Quaternion rotation = Quaternion.Euler(_camPitch, _camYaw, 0f);
            Vector3 offset = rotation * new Vector3(0f, 0f, -_camDistance);

            _cam.transform.position = pivot + offset;
            _cam.transform.LookAt(pivot);
        }

        /// <summary>
        /// Returns true if the debugger has locked this joint's spring.
        /// Other systems (ProceduralStepController) should skip spring changes for locked joints.
        /// </summary>
        public bool IsJointLocked(int jointIndex)
        {
            if (!_enabled || _jointLocked == null) return false;
            if (jointIndex < 0 || jointIndex >= _jointLocked.Length) return false;
            return _jointLocked[jointIndex];
        }

        /// <summary>
        /// Returns true if the walk system is disabled by the debugger.
        /// </summary>
        public bool IsWalkSystemDisabled => _enabled && _walkSystemDisabled;

        private void FixedUpdate()
        {
            if (!_enabled || !_initialized) return;

            // Re-enforce locked joint springs every frame so other systems can't overwrite
            for (int i = 0; i < _jointLocked.Length; i++)
            {
                if (_jointLocked[i])
                    _controller.SetJointSpringMultiplier(i, _springOverrides[i]);
            }

            if (!_isDragging || _selectedRb == null) return;

            var kb = Keyboard.current;
            bool torqueMode = kb != null && kb.leftShiftKey.isPressed;

            if (torqueMode)
            {
                // Torque from mouse delta
                var mouse = Mouse.current;
                if (mouse != null)
                {
                    Vector2 delta = mouse.delta.ReadValue();
                    Vector3 torque = new Vector3(-delta.y, delta.x, 0f) * _torqueForce;
                    _selectedRb.AddTorque(torque, ForceMode.Force);
                }
            }
            else
            {
                // Pull toward mouse target — force must be strong enough to overcome joint springs
                Vector3 toTarget = _dragTarget - _selectedRb.position;
                _selectedRb.AddForce(toTarget * _dragForce, ForceMode.Force);

                // Pin hip so dragging a limb doesn't lift the entire body
                if (_pinHipWhileDragging && _controller.HipRigidbody != null
                    && _selectedRb != _controller.HipRigidbody)
                {
                    var hipRb = _controller.HipRigidbody;
                    // Cancel any upward velocity on hip caused by the drag
                    Vector3 hipVel = hipRb.linearVelocity;
                    if (hipVel.y > 0f)
                        hipRb.linearVelocity = new Vector3(hipVel.x, 0f, hipVel.z);
                    // Strong downward anchor to keep hip grounded
                    hipRb.AddForce(Vector3.down * _dragForce * 0.5f, ForceMode.Force);
                }
            }
        }

        private void TrySelectBone(Vector2 screenPos)
        {
            if (_cam == null) return;

            Ray ray = _cam.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.rigidbody != null && _rbToJointIndex.TryGetValue(hit.rigidbody, out int index)
                    && index >= 0 && index < _controller.AllJoints.Count)
                {
                    _selectedIndex = index;
                    _selectedRb = hit.rigidbody;
                    _selectedJoint = _controller.AllJoints[index];
                    _selectedName = _selectedRb.gameObject.name;
                    _selectionDistance = Vector3.Distance(ray.origin, hit.point);
                    Debug.Log($"[JointDebugger] Selected: {_selectedName} (index {index})");
                }
            }
        }

        private void UpdateDragTarget(Vector2 screenPos)
        {
            if (_cam == null) return;

            _dragTarget = _cam.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, _selectionDistance));
        }

        // ─────────── GIZMOS ───────────

        private void OnDrawGizmos()
        {
            if (!_enabled || !_initialized || _controller == null) return;

            // Draw dots for all joints
            for (int i = 0; i < _controller.AllJoints.Count; i++)
            {
                var joint = _controller.AllJoints[i];
                if (joint == null) continue;

                bool isSelected = i == _selectedIndex;
                if (isSelected) continue; // draw selected separately

                JointLimitGizmoHelper.DrawJointDot(
                    joint.transform.position,
                    new Color(0.5f, 0.5f, 0.5f, 0.6f));
            }

            // Draw selected joint details
            if (_selectedJoint != null && _selectedIndex >= 0)
            {
                Vector3 pos = _selectedJoint.transform.position;

                // Highlight ring
                JointLimitGizmoHelper.DrawHighlightRing(pos, 0.15f, Color.yellow);

                // Joint axes
                JointLimitGizmoHelper.DrawJointAxes(_selectedJoint);

                // Angular limits
                JointLimitGizmoHelper.DrawAngularXLimitArc(_selectedJoint);
                JointLimitGizmoHelper.DrawSwingLimitArcs(_selectedJoint);

                // Drag force line
                if (_isDragging)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(pos, _dragTarget);
                    Gizmos.DrawSphere(_dragTarget, 0.05f);
                }
            }
        }

        // ─────────── OnGUI PANEL ───────────

        private void OnGUI()
        {
            if (!_enabled || !_initialized) return;

            float panelWidth = 320f;
            float panelHeight = 500f;
            float x = 10f;
            float y = 10f;

            GUI.Box(new Rect(x, y, panelWidth, panelHeight), "");

            GUILayout.BeginArea(new Rect(x + 5, y + 5, panelWidth - 10, panelHeight - 10));
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(GUI.skin.label)
                    { fontSize = 14, fontStyle = FontStyle.Bold, normal = { textColor = Color.cyan } };
                _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12 };
                _valueStyle = new GUIStyle(GUI.skin.label)
                    { fontSize = 12, normal = { textColor = Color.green } };
            }
            var headerStyle = _headerStyle;
            var labelStyle = _labelStyle;
            var valueStyle = _valueStyle;

            GUILayout.Label("=== Joint Debugger (RShift+J) ===", headerStyle);
            GUILayout.Label("Click bone to select. Drag to pull. LShift+Drag = torque.", labelStyle);
            GUILayout.Space(8);

            if (_selectedRb == null || _selectedJoint == null)
            {
                GUILayout.Label("No joint selected — click a bone", labelStyle);
                GUILayout.EndScrollView();
                GUILayout.EndArea();
                return;
            }

            // Bone name
            GUILayout.Label($"Bone: {_selectedName}", headerStyle);
            GUILayout.Label($"Joint Index: {_selectedIndex}", labelStyle);
            GUILayout.Space(4);

            // Position & velocity
            GUILayout.Label("── Physics ──", headerStyle);
            GUILayout.Label($"Position: {FormatV3(_selectedRb.position)}", valueStyle);
            GUILayout.Label($"Velocity: {_selectedRb.linearVelocity.magnitude:F2} m/s", valueStyle);
            GUILayout.Label($"Angular Vel: {_selectedRb.angularVelocity.magnitude:F2} rad/s", valueStyle);
            GUILayout.Label($"Mass: {_selectedRb.mass:F2} kg", valueStyle);
            GUILayout.Space(4);

            // Joint drive
            GUILayout.Label("── Joint Drive ──", headerStyle);
            var drive = _selectedJoint.slerpDrive;
            GUILayout.Label($"Spring: {drive.positionSpring:F1}", valueStyle);
            GUILayout.Label($"Damper: {drive.positionDamper:F1}", valueStyle);
            GUILayout.Label($"Max Force: {drive.maximumForce:F1}", valueStyle);
            GUILayout.Space(4);

            // Angular limits
            GUILayout.Label("── Limits ──", headerStyle);
            GUILayout.Label($"X Motion: {_selectedJoint.angularXMotion}", valueStyle);
            if (_selectedJoint.angularXMotion == ConfigurableJointMotion.Limited)
            {
                GUILayout.Label($"  Low X: {_selectedJoint.lowAngularXLimit.limit:F1}°", valueStyle);
                GUILayout.Label($"  High X: {_selectedJoint.highAngularXLimit.limit:F1}°", valueStyle);
            }
            GUILayout.Label($"Y Motion: {_selectedJoint.angularYMotion} (±{_selectedJoint.angularYLimit.limit:F1}°)", valueStyle);
            GUILayout.Label($"Z Motion: {_selectedJoint.angularZMotion} (±{_selectedJoint.angularZLimit.limit:F1}°)", valueStyle);
            GUILayout.Space(4);

            // Rotation
            GUILayout.Label("── Rotation ──", headerStyle);
            GUILayout.Label($"Local Rot: {FormatQ(_selectedJoint.transform.localRotation)}", valueStyle);
            GUILayout.Label($"Target Rot: {FormatQ(_selectedJoint.targetRotation)}", valueStyle);
            GUILayout.Space(8);

            // System toggles
            GUILayout.Label("── System ──", headerStyle);
            if (GUILayout.Button(_walkSystemDisabled ? "Enable Walk System" : "Disable Walk System"))
            {
                _walkSystemDisabled = !_walkSystemDisabled;
                Debug.Log($"[JointDebugger] Walk system: {(_walkSystemDisabled ? "DISABLED" : "ENABLED")}");
            }
            _pinHipWhileDragging = GUILayout.Toggle(_pinHipWhileDragging, " Pin hip while dragging");

            if (GUILayout.Button(_hipFrozen ? "Unfreeze Hip (restore physics)" : "Freeze Hip (lock body in place)"))
            {
                _hipFrozen = !_hipFrozen;
                if (_controller.HipRigidbody != null)
                {
                    var hipRb = _controller.HipRigidbody;
                    hipRb.isKinematic = _hipFrozen;
                    if (!_hipFrozen)
                    {
                        hipRb.linearVelocity = Vector3.zero;
                        hipRb.angularVelocity = Vector3.zero;
                    }
                }
            }
            GUILayout.Space(4);

            // Spring multiplier slider + lock
            GUILayout.Label("── Controls ──", headerStyle);
            bool locked = _jointLocked[_selectedIndex];
            GUILayout.Label($"Spring Multiplier: {_springOverrides[_selectedIndex]:F2} {(locked ? "🔒 LOCKED" : "")}");
            float newSpring = GUILayout.HorizontalSlider(_springOverrides[_selectedIndex], 0f, 2f);
            if (!Mathf.Approximately(newSpring, _springOverrides[_selectedIndex]))
            {
                _springOverrides[_selectedIndex] = newSpring;
                _jointLocked[_selectedIndex] = true;
                _controller.SetJointSpringMultiplier(_selectedIndex, newSpring);
            }
            GUILayout.Space(4);

            // Impulse buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Up"))
                _selectedRb.AddForce(Vector3.up * _impulseStrength, ForceMode.Impulse);
            if (GUILayout.Button("Fwd"))
                _selectedRb.AddForce(_selectedRb.transform.forward * _impulseStrength, ForceMode.Impulse);
            if (GUILayout.Button("Rotate"))
                _selectedRb.AddTorque(Vector3.up * _impulseStrength * 2f, ForceMode.Impulse);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Zero Vel"))
            {
                _selectedRb.linearVelocity = Vector3.zero;
                _selectedRb.angularVelocity = Vector3.zero;
            }
            if (GUILayout.Button("Go Limp"))
            {
                _springOverrides[_selectedIndex] = 0f;
                _jointLocked[_selectedIndex] = true;
                _controller.SetJointSpringMultiplier(_selectedIndex, 0f);
            }
            if (GUILayout.Button("Full Spring"))
            {
                _springOverrides[_selectedIndex] = 1f;
                _jointLocked[_selectedIndex] = false; // unlock on restore
                _controller.SetJointSpringMultiplier(_selectedIndex, 1f);
            }
            GUILayout.EndHorizontal();

            // Unlock button
            if (locked)
            {
                if (GUILayout.Button("Unlock Joint (let walk system control)"))
                {
                    _jointLocked[_selectedIndex] = false;
                    _springOverrides[_selectedIndex] = 1f;
                    _controller.SetJointSpringMultiplier(_selectedIndex, 1f);
                }
            }

            // Chain operations — limp/restore connected joints (whole limb)
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Limp Chain"))
            {
                SetChainSpring(_selectedJoint, 0f, true);
            }
            if (GUILayout.Button("Restore Chain"))
            {
                SetChainSpring(_selectedJoint, 1f, false);
            }
            if (GUILayout.Button("Limp ALL"))
            {
                for (int i = 0; i < _controller.JointCount; i++)
                {
                    _springOverrides[i] = 0f;
                    _jointLocked[i] = true;
                    _controller.SetJointSpringMultiplier(i, 0f);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            // All joints quick list
            GUILayout.Label("── All Joints ──", headerStyle);
            for (int i = 0; i < _controller.AllJoints.Count; i++)
            {
                var j = _controller.AllJoints[i];
                if (j == null) continue;

                string name = j.gameObject.name;
                bool selected = i == _selectedIndex;
                string prefix = selected ? "► " : "  ";
                float spring = _controller.GetJointSpringMultiplier(i);

                if (GUILayout.Button($"{prefix}{name} (spring: {spring:F2})", selected ? headerStyle : labelStyle))
                {
                    _selectedIndex = i;
                    _selectedRb = j.GetComponent<Rigidbody>();
                    _selectedJoint = j;
                    _selectedName = j.gameObject.name;
                    if (_selectedRb != null && _cam != null)
                        _selectionDistance = Vector3.Distance(_cam.transform.position, _selectedRb.position);
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Set spring multiplier on a joint and all its child joints (the whole limb chain).
        /// Walks down the hierarchy: thigh → shin → foot.
        /// </summary>
        private void SetChainSpring(ConfigurableJoint startJoint, float spring, bool lockJoints)
        {
            // Find all joints whose rigidbody is a child (direct or nested) of the start joint's transform
            Transform startBone = startJoint.transform;
            for (int i = 0; i < _controller.AllJoints.Count; i++)
            {
                var joint = _controller.AllJoints[i];
                if (joint == null) continue;

                // Check if this joint's transform is the start bone or a descendant of it
                Transform t = joint.transform;
                while (t != null)
                {
                    if (t == startBone)
                    {
                        _springOverrides[i] = spring;
                        _jointLocked[i] = lockJoints;
                        _controller.SetJointSpringMultiplier(i, spring);
                        break;
                    }
                    t = t.parent;
                }
            }
        }

        private static string FormatV3(Vector3 v) =>
            $"({v.x:+0.00;-0.00}, {v.y:+0.00;-0.00}, {v.z:+0.00;-0.00})";

        private static string FormatQ(Quaternion q) =>
            $"({q.x:+0.00;-0.00}, {q.y:+0.00;-0.00}, {q.z:+0.00;-0.00}, {q.w:+0.00;-0.00})";
    }
}
