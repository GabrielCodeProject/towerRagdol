using UnityEngine;
using RagdollRealms.Core;

namespace RagdollRealms.Systems.Ragdoll
{
    /// <summary>
    /// Keeps the active ragdoll standing using the puppet/marionette approach:
    /// 1. Partial anti-gravity on hip (90%) — character stays light but still settles on ground
    /// 2. Uprighting torque — prevents the hip from spinning/tipping over
    /// 3. Angular drag — dampens rotational oscillation
    ///
    /// Foundation for a future full balance controller.
    /// </summary>
    public class RagdollBalanceForce : MonoBehaviour
    {
        [Header("Balance Tuning")]
        [SerializeField] private float _gravityCompensation = 0.9f;
        [SerializeField] private float _uprightTorque = 50f;
        [SerializeField] private float _angularDrag = 5f;

        private IRagdollController _controller;
        private Rigidbody _hipRb;
        private float _totalMass;
        private float _originalAngularDrag;
        private bool _initialized;

        private void Start()
        {
            _controller = GetComponent<IRagdollController>();
            if (_controller == null) return;

            _hipRb = _controller.HipRigidbody;
            if (_hipRb == null) return;

            // Sum total mass of all ragdoll bodies
            _totalMass = 0f;
            foreach (var body in _controller.AllBodies)
                _totalMass += body.mass;

            // Increase angular drag on hip to prevent spinning
            _originalAngularDrag = _hipRb.angularDamping;
            _hipRb.angularDamping = _angularDrag;

            _initialized = true;
        }

        private void FixedUpdate()
        {
            if (!_initialized || _controller.IsRagdolling) return;

            // 1. Partial anti-gravity: cancel 90% of gravity so character is light
            //    but still settles on ground via colliders
            Vector3 antiGravity = -Physics.gravity * _totalMass * _gravityCompensation;
            _hipRb.AddForce(antiGravity, ForceMode.Force);

            // 2. Uprighting torque: align hip rotation with the root transform.
            //    PlayerMovementController rotates the root (transform.parent or transform),
            //    so the hip must follow that orientation — not just world up.
            Quaternion targetRotation = transform.rotation;
            Quaternion currentRotation = _hipRb.transform.rotation;
            Quaternion delta = targetRotation * Quaternion.Inverse(currentRotation);
            delta.ToAngleAxis(out float angle, out Vector3 axis);

            // Normalize angle to -180..180
            if (angle > 180f) angle -= 360f;

            if (Mathf.Abs(angle) > 0.1f)
            {
                Vector3 torque = axis.normalized * Mathf.Clamp(angle, -45f, 45f) * _uprightTorque;
                _hipRb.AddTorque(torque, ForceMode.Force);
            }
        }

        private void OnDestroy()
        {
            if (_hipRb != null)
                _hipRb.angularDamping = _originalAngularDrag;
        }
    }
}
