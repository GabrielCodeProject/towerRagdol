using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;

namespace RagdollRealms.Systems.Ragdoll
{
    /// <summary>
    /// Central controller for per-limb collision wobble.
    /// Receives collision reports from BoneCollisionReporters, temporarily reduces
    /// joint spring on impacted limbs, and auto-recovers over time.
    /// </summary>
    public class RagdollCollisionHandler : MonoBehaviour
    {
        private const float ImpactNormalizationScale = 5f;

        private IRagdollController _controller;
        private AnimationFollower _animFollower;
        private IEventBus _eventBus;
        private IRagdollConfig _config;

        private float[] _impactWeights;
        private List<int>[] _adjacentJoints;
        private bool _initialized;

        private void Start()
        {
            _controller = GetComponent<IRagdollController>();
            _animFollower = GetComponent<AnimationFollower>();

            if (_controller == null) return;

            _config = _controller.Config;

            if (ServiceLocator.Instance != null && ServiceLocator.Instance.TryGet<IEventBus>(out var bus))
                _eventBus = bus;

            int count = _controller.JointCount;
            _impactWeights = new float[count];
            BuildAdjacencyMap();
            _initialized = true;
        }

        public void ReportBoneCollision(int jointIndex, Collision collision)
        {
            float forceMagnitude = collision.impulse.magnitude;
            Vector3 contactPoint = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
            ApplyImpact(jointIndex, forceMagnitude, collision.impulse, contactPoint);
        }

        /// <summary>
        /// Apply a manual impact to a joint (for scripts, spells, or testing).
        /// Does not require a physics Collision — works with any force value.
        /// </summary>
        public void ApplyImpact(int jointIndex, float forceMagnitude, Vector3 force = default, Vector3 contactPoint = default)
        {
            if (!_initialized || _config == null) return;
            if (jointIndex < 0 || jointIndex >= _impactWeights.Length) return;
            if (forceMagnitude < _config.MinCollisionForce) return;

            // Normalize impact: stronger forces = higher weight, capped at 1
            float threshold = Mathf.Max(_config.MinCollisionForce, 0.01f);
            float impactWeight = Mathf.Clamp01(forceMagnitude / (threshold * ImpactNormalizationScale));

            // Apply to impacted joint (keep the stronger of existing vs new)
            _impactWeights[jointIndex] = Mathf.Max(_impactWeights[jointIndex], impactWeight);

            // Propagate to adjacent joints
            if (_adjacentJoints != null && jointIndex < _adjacentJoints.Length && _adjacentJoints[jointIndex] != null)
            {
                float propagated = impactWeight * _config.CollisionPropagationFactor;
                foreach (int adj in _adjacentJoints[jointIndex])
                {
                    _impactWeights[adj] = Mathf.Max(_impactWeights[adj], propagated);
                }
            }

            // Apply blend weight reduction to AnimationFollower
            ApplyBlendWeights();

            // Publish event
            _eventBus?.Publish(new OnBoneCollisionImpact(
                gameObject.GetInstanceID(),
                jointIndex,
                force,
                contactPoint,
                forceMagnitude
            ));
        }

        private void FixedUpdate()
        {
            if (!_initialized) return;

            bool anyActive = false;
            float recoveryStep = _config.CollisionRecoverySpeed * Time.fixedDeltaTime;

            for (int i = 0; i < _impactWeights.Length; i++)
            {
                if (_impactWeights[i] > 0f)
                {
                    _impactWeights[i] = Mathf.MoveTowards(_impactWeights[i], 0f, recoveryStep);
                    anyActive = true;
                }
            }

            if (anyActive)
            {
                ApplyBlendWeights();
            }
        }

        private void ApplyBlendWeights()
        {
            if (_animFollower == null) return;

            for (int i = 0; i < _impactWeights.Length; i++)
            {
                float blendWeight = 1f - (_impactWeights[i] * _config.CollisionSpringReduction);
                _animFollower.SetJointBlendWeight(i, blendWeight);
            }
        }

        private void BuildAdjacencyMap()
        {
            int count = _controller.JointCount;
            _adjacentJoints = new List<int>[count];

            for (int i = 0; i < count; i++)
                _adjacentJoints[i] = new List<int>();

            var joints = _controller.AllJoints;
            for (int i = 0; i < count; i++)
            {
                var joint = joints[i];
                if (joint.connectedBody == null) continue;

                // Find the index of the connected (parent) joint
                var parentJoint = joint.connectedBody.GetComponent<ConfigurableJoint>();
                if (parentJoint == null) continue;

                int parentIndex = _controller.GetJointIndex(parentJoint);
                if (parentIndex < 0) continue;

                // Bidirectional adjacency
                if (!_adjacentJoints[i].Contains(parentIndex))
                    _adjacentJoints[i].Add(parentIndex);
                if (!_adjacentJoints[parentIndex].Contains(i))
                    _adjacentJoints[parentIndex].Add(i);
            }
        }
    }
}
