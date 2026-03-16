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
        private Coroutine _healingCoroutine;

        private static readonly Collider[] _overlapBuffer = new Collider[8];

        private void Start()
        {
            if (!ServiceLocator.Instance.TryGet(out _eventBus))
            {
                Debug.LogError("[CoreHealingAura] IEventBus not registered.");
                return;
            }

            if (!ServiceLocator.Instance.TryGet(out _coreManager))
            {
                Debug.LogError("[CoreHealingAura] ICoreManager not registered.");
                return;
            }

            _effectiveRadius = _coreManager.HealingAuraRadius;
            _effectiveRate = _coreManager.HealingAuraRate;

            UpdateAuraVisual();

            _onCoreUpgraded = HandleCoreUpgraded;
            _eventBus.Subscribe(_onCoreUpgraded);

            _healingCoroutine = StartCoroutine(HealingTickRoutine());
        }

        private IEnumerator HealingTickRoutine()
        {
            var wait = new WaitForSeconds(_tickInterval);
            while (true)
            {
                yield return wait;

                if (_coreManager is MonoBehaviour mb && mb == null) yield break;
                if (_coreManager.IsDestroyed) continue;

                int count = Physics.OverlapSphereNonAlloc(
                    transform.position, _effectiveRadius, _overlapBuffer, _playerLayerMask);

                float healAmount = _effectiveRate * _tickInterval;
                for (int i = 0; i < count; i++)
                {
                    var playerCtrl = _overlapBuffer[i].transform.root.GetComponent<IPlayerController>();
                    if (playerCtrl == null) continue;
                    _eventBus.Publish(new OnCoreHealRequest(playerCtrl.PlayerId, healAmount));
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
            if (_healingCoroutine != null)
                StopCoroutine(_healingCoroutine);

            if (_eventBus != null)
                _eventBus.Unsubscribe(_onCoreUpgraded);
        }
    }
}
