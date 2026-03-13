using UnityEngine;
using RagdollRealms.Core;

namespace RagdollRealms.Systems.Ragdoll
{
    public class AnimationFollower : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform[] _animatedBones;

        private IRagdollController _controller;
        private Quaternion[] _initialRotations;
        private Quaternion[] _inverseInitialRotations;
        private bool _enabled = true;
        private float _blendWeight = 1f;

        public bool IsEnabled => _enabled;

        public float BlendWeight
        {
            get => _blendWeight;
            set => _blendWeight = Mathf.Clamp01(value);
        }

        private bool _initialized;

        private void Start()
        {
            InitializeIfNeeded();
        }

        private void InitializeIfNeeded()
        {
            if (_initialized) return;

            _controller = GetComponent<IRagdollController>();
            if (_controller == null || _controller.AllJoints == null) return;

            var joints = _controller.AllJoints;
            _initialRotations = new Quaternion[joints.Count];
            _inverseInitialRotations = new Quaternion[joints.Count];

            for (int i = 0; i < joints.Count; i++)
            {
                _initialRotations[i] = joints[i].transform.localRotation;
                _inverseInitialRotations[i] = Quaternion.Inverse(_initialRotations[i]);
            }

            _initialized = true;
        }

        private void FixedUpdate()
        {
            if (!_enabled || !_initialized || _controller == null) return;

            var joints = _controller.AllJoints;

            for (int i = 0; i < joints.Count; i++)
            {
                if (i >= _animatedBones.Length || _animatedBones[i] == null) continue;

                var targetRot = _inverseInitialRotations[i] * _animatedBones[i].localRotation;
                joints[i].targetRotation = targetRot;
            }

            _controller.SetJointSpringMultiplier(_blendWeight);
        }

        public void SetEnabled(bool enabled)
        {
            _enabled = enabled;
        }
    }
}
