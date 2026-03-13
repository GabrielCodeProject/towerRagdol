using UnityEngine;
using RagdollRealms.Core;

namespace RagdollRealms.Systems.Ragdoll
{
    public class RagdollLODController : MonoBehaviour
    {
        private enum RagdollLOD { Full, Reduced, Disabled }

        [SerializeField] private int _checkInterval = 10;

        private IRagdollController _controller;
        private AnimationFollower _animFollower;
        private IPerformanceBudget _budget;
        private RagdollLOD _currentLOD = RagdollLOD.Full;
        private bool _isTracked;
        private Transform _cachedCamera;
        private int _frameCounter;

        private void Start()
        {
            _controller = GetComponent<IRagdollController>();
            _animFollower = GetComponent<AnimationFollower>();
            if (ServiceLocator.Instance != null && ServiceLocator.Instance.TryGet<IPerformanceBudget>(out var budget))
                _budget = budget;
            _cachedCamera = Camera.main?.transform;
        }

        private void OnEnable()
        {
            if (_controller != null)
                ApplyLOD(_currentLOD);
        }

        private void OnDisable()
        {
            UntrackIfNeeded();
        }

        private void Update()
        {
            if (_controller == null || _budget == null) return;

            _frameCounter = (_frameCounter + 1) % _checkInterval;
            if (_frameCounter != 0) return;

            if (_cachedCamera == null)
            {
                _cachedCamera = Camera.main?.transform;
                return;
            }

            float distance = Vector3.Distance(_cachedCamera.position, transform.position);
            var config = _controller.Config;

            RagdollLOD targetLOD;
            if (distance <= config.LodFullDistance)
                targetLOD = RagdollLOD.Full;
            else if (distance <= config.LodReducedDistance)
                targetLOD = RagdollLOD.Reduced;
            else
                targetLOD = RagdollLOD.Disabled;

            if (targetLOD == RagdollLOD.Full && !_budget.CanSpawnRagdoll())
                targetLOD = RagdollLOD.Reduced;

            if (targetLOD != _currentLOD)
                ApplyLOD(targetLOD);
        }

        private void ApplyLOD(RagdollLOD level)
        {
            switch (level)
            {
                case RagdollLOD.Full:
                    _controller.SetKinematic(false);
                    _controller.SetJointSpringMultiplier(1f);
                    _animFollower?.SetEnabled(true);
                    TrackIfNeeded();
                    break;

                case RagdollLOD.Reduced:
                    _controller.SetKinematic(false);
                    _controller.SetJointSpringMultiplier(0.5f);
                    _animFollower?.SetEnabled(true);
                    TrackIfNeeded();
                    break;

                case RagdollLOD.Disabled:
                    _controller.SetKinematic(true);
                    _animFollower?.SetEnabled(false);
                    UntrackIfNeeded();
                    break;
            }

            _currentLOD = level;
        }

        private void TrackIfNeeded()
        {
            if (!_isTracked)
            {
                _budget.TrackRagdoll(1);
                _isTracked = true;
            }
        }

        private void UntrackIfNeeded()
        {
            if (_isTracked)
            {
                _budget.TrackRagdoll(-1);
                _isTracked = false;
            }
        }
    }
}
