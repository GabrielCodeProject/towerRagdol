using UnityEngine;

namespace RagdollRealms.Systems.Ragdoll
{
    /// <summary>
    /// Ground contact sensor attached to a foot rigidbody at runtime.
    /// Uses collision contacts as primary detection + raycast fallback.
    /// Also adds friction to the foot collider for better grip.
    /// </summary>
    public class FootSensor : MonoBehaviour
    {
        private float _checkDistance = 0.3f;
        private LayerMask _groundMask;
        private Rigidbody _rb;
        private PhysicsMaterial _footMaterial;
        private int _contactCount;
        private Vector3 _contactPoint;
        private Vector3 _contactNormal;

        public bool IsGrounded { get; private set; }
        public Vector3 GroundPoint { get; private set; }
        public Vector3 GroundNormal { get; private set; }
        public Rigidbody Rigidbody => _rb;

        public void Initialize(float checkDistance, LayerMask groundMask)
        {
            _checkDistance = Mathf.Max(checkDistance, 0.3f);
            _groundMask = groundMask;
            _rb = GetComponent<Rigidbody>();

            // Add high-friction physics material to foot for grip
            var collider = GetComponent<Collider>();
            if (collider != null)
            {
                _footMaterial = new PhysicsMaterial("FootGrip")
                {
                    staticFriction = 1.2f,
                    dynamicFriction = 1.0f,
                    frictionCombine = PhysicsMaterialCombine.Maximum
                };
                collider.material = _footMaterial;
            }
        }

        private void FixedUpdate()
        {
            if (_rb == null) return;

            // Primary: collision contacts (set by OnCollisionStay)
            if (_contactCount > 0)
            {
                IsGrounded = true;
                GroundPoint = _contactPoint;
                GroundNormal = _contactNormal;
            }
            // Fallback: raycast down (filtered by ground layer mask)
            else if (Physics.Raycast(_rb.position, Vector3.down, out var hit, _checkDistance, _groundMask))
            {
                IsGrounded = true;
                GroundPoint = hit.point;
                GroundNormal = hit.normal;
            }
            else
            {
                IsGrounded = false;
            }

            _contactCount = 0;
        }

        private void OnDestroy()
        {
            if (_footMaterial != null)
                Destroy(_footMaterial);
        }

        private void OnCollisionStay(Collision collision)
        {
            // Only count contacts below the foot (ground-like surfaces)
            for (int i = 0; i < collision.contactCount; i++)
            {
                var contact = collision.GetContact(i);
                if (contact.normal.y > 0.5f) // surface is mostly upward-facing
                {
                    _contactCount++;
                    _contactPoint = contact.point;
                    _contactNormal = contact.normal;
                    return;
                }
            }
        }
    }
}
