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
        private float[] _perJointMultipliers;
        private float _globalMultiplier = 1f;
        private Dictionary<ConfigurableJoint, int> _jointIndexMap;

        public Rigidbody HipRigidbody => _hipRigidbody;
        public IReadOnlyList<ConfigurableJoint> AllJoints => _allJoints;
        public IReadOnlyList<Rigidbody> AllBodies => _allBodies;
        public bool IsRagdolling { get; set; }
        public IRagdollConfig Config => _config;
        public int JointCount => _allJoints.Length;

        private void Awake()
        {
            _allJoints = GetComponentsInChildren<ConfigurableJoint>();
            _allBodies = GetComponentsInChildren<Rigidbody>();
            // Initialize per-joint multiplier tracking
            _perJointMultipliers = new float[_allJoints.Length];
            _jointIndexMap = new Dictionary<ConfigurableJoint, int>(_allJoints.Length);
            for (int i = 0; i < _allJoints.Length; i++)
            {
                _perJointMultipliers[i] = 1f;
                _jointIndexMap[_allJoints[i]] = i;
            }
            // Apply joint drive settings at startup so ragdoll can stand
            SetJointSpringMultiplier(1f);
        }

        public void SetJointSpringMultiplier(float multiplier)
        {
            _globalMultiplier = multiplier;
            ApplyAllJointDrives();
        }

        public void SetJointSpringMultiplier(int jointIndex, float multiplier)
        {
            if (jointIndex < 0 || jointIndex >= _allJoints.Length) return;
            _perJointMultipliers[jointIndex] = multiplier;
            ApplyJointDrive(jointIndex);
        }

        public float GetJointSpringMultiplier(int jointIndex)
        {
            if (jointIndex < 0 || jointIndex >= _allJoints.Length) return 0f;
            return _perJointMultipliers[jointIndex];
        }

        public int GetJointIndex(ConfigurableJoint joint)
        {
            return _jointIndexMap.TryGetValue(joint, out int index) ? index : -1;
        }

        private void ApplyJointDrive(int index)
        {
            float effectiveMultiplier = _globalMultiplier * _perJointMultipliers[index];
            var drive = new JointDrive
            {
                positionSpring = _config.DefaultSpring * effectiveMultiplier,
                // Scale damper with spring — keeps the system underdamped so
                // impacted joints oscillate/wobble instead of smoothly returning.
                positionDamper = _config.DefaultDamper * effectiveMultiplier,
                maximumForce = _config.MaxSpringForce * Mathf.Max(effectiveMultiplier, 0.01f)
            };
            _allJoints[index].angularXDrive = drive;
            _allJoints[index].angularYZDrive = drive;
            _allJoints[index].slerpDrive = drive;
        }

        private void ApplyAllJointDrives()
        {
            for (int i = 0; i < _allJoints.Length; i++)
            {
                ApplyJointDrive(i);
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
            ResetMultipliers();
            ApplyAllJointDrives();
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
            ResetMultipliers();
        }

        private void ResetMultipliers()
        {
            _globalMultiplier = 1f;
            for (int i = 0; i < _perJointMultipliers.Length; i++)
                _perJointMultipliers[i] = 1f;
        }
    }
}
