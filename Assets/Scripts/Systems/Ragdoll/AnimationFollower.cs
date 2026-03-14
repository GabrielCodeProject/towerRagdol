using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;

namespace RagdollRealms.Systems.Ragdoll
{
    /// <summary>
    /// Active ragdoll muscle system — dual skeleton pattern.
    ///
    /// Creates a hidden duplicate skeleton driven by its own Animator.
    /// Each FixedUpdate, reads the duplicate's bone rotations and sets them
    /// as ConfigurableJoint targets on the physics skeleton.
    ///
    /// Based on the CopyMotion + ConfigurableJointExtensions pattern from
    /// working active ragdoll implementations (rmguney/active-ragdolls,
    /// mstevenson/ConfigurableJointExtensions).
    /// </summary>
    public class AnimationFollower : MonoBehaviour
    {
        private IRagdollController _controller;
        private Animator _sourceAnimator;
        private Animator _targetAnimator;
        private GameObject _animSkeleton;

        private ConfigurableJoint[] _joints;
        private Transform[] _targetBones;
        private Quaternion[] _startLocalRotations;
        private Quaternion[] _worldToJointSpace;
        private Quaternion[] _inverseWorldToJointSpace;

        private bool _enabled = true;
        private float _blendWeight = 1f;
        private bool _initialized;
        private Coroutine _warmupCoroutine;

        private IEventBus _eventBus;
        private Action<OnPlayerSpeedChanged> _onSpeedChanged;

        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        public bool IsEnabled => _enabled;
        public Animator TargetAnimator => _targetAnimator;

        public float BlendWeight
        {
            get => _blendWeight;
            set => _blendWeight = Mathf.Clamp01(value);
        }

        private void Start()
        {
            InitializeIfNeeded();
        }

        private void InitializeIfNeeded()
        {
            if (_initialized) return;

            _controller = GetComponent<IRagdollController>();
            if (_controller == null || _controller.AllJoints == null) return;

            _sourceAnimator = GetComponent<Animator>();
            if (_sourceAnimator == null) return;

            if (ServiceLocator.Instance != null && ServiceLocator.Instance.TryGet(out _eventBus))
            {
                _onSpeedChanged = HandleSpeedChanged;
                _eventBus.Subscribe(_onSpeedChanged);
            }

            var jointList = _controller.AllJoints;
            int count = jointList.Count;

            _joints = new ConfigurableJoint[count];
            _startLocalRotations = new Quaternion[count];
            _worldToJointSpace = new Quaternion[count];
            _inverseWorldToJointSpace = new Quaternion[count];

            for (int i = 0; i < count; i++)
            {
                _joints[i] = jointList[i];

                // Cache the starting local rotation of each physics bone
                _startLocalRotations[i] = _joints[i].transform.localRotation;

                // Calculate the joint's coordinate space from its axis configuration
                // (from ConfigurableJointExtensions by mstevenson)
                Vector3 right = _joints[i].axis;
                Vector3 forward = Vector3.Cross(_joints[i].axis, _joints[i].secondaryAxis).normalized;
                Vector3 up = Vector3.Cross(forward, right).normalized;
                _worldToJointSpace[i] = Quaternion.LookRotation(forward, up);
                _inverseWorldToJointSpace[i] = Quaternion.Inverse(_worldToJointSpace[i]);
            }

            BuildAnimationSkeleton();

            // Disable source Animator — physics joints are the sole bone controller.
            _sourceAnimator.enabled = false;

            _initialized = true;

            // Hold ragdoll kinematic while animation skeleton warms up,
            // then release to physics once joint targets are valid.
            _warmupCoroutine = StartCoroutine(WarmupAndRelease());
        }

        private void BuildAnimationSkeleton()
        {
            _animSkeleton = new GameObject("_AnimSkeleton");
            _animSkeleton.transform.SetParent(transform.parent);
            _animSkeleton.transform.position = transform.position;
            _animSkeleton.transform.rotation = transform.rotation;
            _animSkeleton.transform.localScale = transform.localScale;

            var boneMap = new Dictionary<Transform, Transform>();
            boneMap[transform] = _animSkeleton.transform;

            DuplicateHierarchy(transform, _animSkeleton.transform, boneMap);

            _targetAnimator = _animSkeleton.AddComponent<Animator>();
            _targetAnimator.runtimeAnimatorController = _sourceAnimator.runtimeAnimatorController;
            _targetAnimator.avatar = _sourceAnimator.avatar;
            _targetAnimator.applyRootMotion = false;
            _targetAnimator.updateMode = AnimatorUpdateMode.Normal;
            _targetAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            _targetBones = new Transform[_joints.Length];
            for (int i = 0; i < _joints.Length; i++)
            {
                if (boneMap.TryGetValue(_joints[i].transform, out var dup))
                    _targetBones[i] = dup;
            }

            _animSkeleton.hideFlags = HideFlags.HideInHierarchy;
        }

        private void DuplicateHierarchy(Transform source, Transform dupParent,
            Dictionary<Transform, Transform> map)
        {
            foreach (Transform child in source)
            {
                var dupChild = new GameObject(child.name).transform;
                dupChild.SetParent(dupParent);
                dupChild.localPosition = child.localPosition;
                dupChild.localRotation = child.localRotation;
                dupChild.localScale = child.localScale;
                map[child] = dupChild;
                DuplicateHierarchy(child, dupChild, map);
            }
        }

        private void FixedUpdate()
        {
            if (!_enabled || !_initialized || _controller == null) return;

            // Keep duplicate skeleton root in sync with physics root
            _animSkeleton.transform.position = transform.position;
            _animSkeleton.transform.rotation = transform.rotation;

            // Apply animation targets using proper joint-space conversion
            for (int i = 0; i < _joints.Length; i++)
            {
                if (_targetBones[i] == null) continue;
                SetTargetRotationLocal(
                    _joints[i], i,
                    _targetBones[i].localRotation
                );
            }

            _controller.SetJointSpringMultiplier(_blendWeight);
        }

        /// <summary>
        /// Converts a desired local rotation to ConfigurableJoint target space.
        /// Based on ConfigurableJointExtensions by mstevenson (proven, widely used).
        /// </summary>
        private void SetTargetRotationLocal(ConfigurableJoint joint, int index,
            Quaternion targetLocalRotation)
        {
            // Convert from local space to joint space:
            // result = Inverse(worldToJoint) * Inverse(target) * start * worldToJoint
            joint.targetRotation =
                _inverseWorldToJointSpace[index] *
                Quaternion.Inverse(targetLocalRotation) *
                _startLocalRotations[index] *
                _worldToJointSpace[index];
        }

        private IEnumerator WarmupAndRelease()
        {
            // Hold kinematic so ragdoll doesn't fall while targets initialize
            _controller.SetKinematic(true);

            // Wait for animation skeleton to evaluate a few frames
            for (int i = 0; i < 10; i++)
                yield return new WaitForFixedUpdate();

            _warmupCoroutine = null;

            // Release to physics — joints now have valid targets
            if (_controller != null)
                _controller.SetKinematic(false);
        }

        private void HandleSpeedChanged(OnPlayerSpeedChanged evt)
        {
            if (_targetAnimator != null)
                _targetAnimator.SetFloat(SpeedHash, evt.Speed);
            if (_sourceAnimator != null)
                _sourceAnimator.SetFloat(SpeedHash, evt.Speed);
        }

        public void SetEnabled(bool enabled)
        {
            _enabled = enabled;
        }

        private void OnDestroy()
        {
            if (_warmupCoroutine != null)
                StopCoroutine(_warmupCoroutine);

            if (_eventBus != null && _onSpeedChanged != null)
                _eventBus.Unsubscribe(_onSpeedChanged);

            if (_animSkeleton != null)
                Destroy(_animSkeleton);
        }
    }
}
