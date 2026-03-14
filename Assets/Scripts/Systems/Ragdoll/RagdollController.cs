using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Content;
using RagdollRealms.Core;

namespace RagdollRealms.Systems.Ragdoll
{
    public class RagdollController : MonoBehaviour, IRagdollController, ISpawnable
    {
        [SerializeField] private RagdollConfigDefinition _config;
        [SerializeField] private Rigidbody _hipRigidbody;

        private ConfigurableJoint[] _allJoints;
        private Rigidbody[] _allBodies;

        public Rigidbody HipRigidbody => _hipRigidbody;
        public IReadOnlyList<ConfigurableJoint> AllJoints => _allJoints;
        public IReadOnlyList<Rigidbody> AllBodies => _allBodies;
        public bool IsRagdolling { get; set; }
        public IRagdollConfig Config => _config;

        private void Awake()
        {
            _allJoints = GetComponentsInChildren<ConfigurableJoint>();
            _allBodies = GetComponentsInChildren<Rigidbody>();
            // Apply joint drive settings at startup so ragdoll can stand
            SetJointSpringMultiplier(1f);
        }

        public void SetJointSpringMultiplier(float multiplier)
        {
            var drive = new JointDrive
            {
                positionSpring = _config.DefaultSpring * multiplier,
                positionDamper = _config.DefaultDamper,
                maximumForce = _config.MaxSpringForce
            };

            foreach (var joint in _allJoints)
            {
                joint.angularXDrive = drive;
                joint.angularYZDrive = drive;
                joint.slerpDrive = drive;
            }
        }

        public void SetKinematic(bool kinematic)
        {
            foreach (var body in _allBodies)
            {
                body.isKinematic = kinematic;
            }
        }

        public void Initialize()
        {
            SetJointSpringMultiplier(1f);
            IsRagdolling = false;
            SetKinematic(false);
        }

        public void Reset()
        {
            SetKinematic(true);
            foreach (var body in _allBodies)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }
        }
    }
}
