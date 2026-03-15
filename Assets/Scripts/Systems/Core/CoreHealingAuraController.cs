using System;
using System.Collections;
using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;

namespace RagdollRealms.Systems.CoreBase
{
    public sealed class CoreHealingAuraController : MonoBehaviour
    {
        [SerializeField] private float _tickInterval = 0.5f;
        [SerializeField] private LayerMask _playerLayerMask;
        [SerializeField] private ParticleSystem _auraParticles;

        private float _effectiveRadius;
        private float _effectiveRate;
        private IEventBus _eventBus;
        private ICoreManager _coreManager;
        private Action<OnCoreUpgraded> _onCoreUpgraded;

        private static readonly Collider[] _overlapBuffer = new Collider[8];

        private void Start()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();
            _coreManager = ServiceLocator.Instance.Get<ICoreManager>();

            _effectiveRadius = _coreManager.HealingAuraRadius;
            _effectiveRate = _coreManager.HealingAuraRate;

            UpdateAuraVisual();

            _onCoreUpgraded = HandleCoreUpgraded;
            _eventBus.Subscribe(_onCoreUpgraded);

            StartCoroutine(HealingTickRoutine());
        }

        private IEnumerator HealingTickRoutine()
        {
            var wait = new WaitForSeconds(_tickInterval);
            while (true)
            {
                yield return wait;
                if (_coreManager == null || _coreManager.IsDestroyed) continue;

                int count = Physics.OverlapSphereNonAlloc(
                    transform.position, _effectiveRadius, _overlapBuffer, _playerLayerMask);

                for (int i = 0; i < count; i++)
                {
                    // Actual player healing depends on the player health system.
                    // A future PlayerHealthManager will consume heal requests.
                }
            }
        }

        private void HandleCoreUpgraded(OnCoreUpgraded evt)
        {
            if (_coreManager == null) return;

            // SSOT: Read calculated values from CoreManager (single source)
            _effectiveRadius = _coreManager.HealingAuraRadius;
            _effectiveRate = _coreManager.HealingAuraRate;
            UpdateAuraVisual();
        }

        private void UpdateAuraVisual()
        {
            if (_auraParticles == null) return;

            var shape = _auraParticles.shape;
            shape.radius = _effectiveRadius;
        }

        private void OnDestroy()
        {
            if (_eventBus != null)
                _eventBus.Unsubscribe(_onCoreUpgraded);
        }
    }
}
