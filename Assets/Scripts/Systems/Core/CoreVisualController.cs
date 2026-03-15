using System;
using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;

namespace RagdollRealms.Systems.CoreBase
{
    public sealed class CoreVisualController : MonoBehaviour
    {
        [Header("Crack Stages")]
        [SerializeField] private GameObject[] _crackStages;

        [Header("Glow")]
        [SerializeField] private Light _glowLight;
        [SerializeField] private float _baseGlowIntensity = 2f;
        [SerializeField] private float _glowIntensityPerTier = 0.5f;
        [SerializeField] private ParticleSystem _glowParticles;

        [Header("Shield")]
        [SerializeField] private GameObject _shieldBubble;
        [SerializeField] private Renderer _shieldRenderer;

        [Header("Upgrade")]
        [SerializeField] private Transform _coreModel;
        [SerializeField] private float _scalePerTier = 0.05f;

        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _damageAlarmClip;

        private IEventBus _eventBus;
        private Action<OnCoreHealthChanged> _onHealthChanged;
        private Action<OnCoreShieldChanged> _onShieldChanged;
        private Action<OnCoreUpgraded> _onUpgraded;
        private float _previousHp = float.MaxValue;
        private Material _shieldMaterial;

        private void Start()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();

            _onHealthChanged = HandleHealthChanged;
            _onShieldChanged = HandleShieldChanged;
            _onUpgraded = HandleUpgraded;

            _eventBus.Subscribe(_onHealthChanged);
            _eventBus.Subscribe(_onShieldChanged);
            _eventBus.Subscribe(_onUpgraded);

            if (_shieldRenderer != null)
                _shieldMaterial = _shieldRenderer.material;

            if (_crackStages != null)
            {
                foreach (var stage in _crackStages)
                    if (stage != null) stage.SetActive(false);
            }
        }

        private void HandleHealthChanged(OnCoreHealthChanged evt)
        {
            float ratio = evt.MaxHp > 0f ? evt.CurrentHp / evt.MaxHp : 0f;

            if (_crackStages != null && _crackStages.Length > 0)
            {
                int stageCount = _crackStages.Length;
                for (int i = 0; i < stageCount; i++)
                {
                    if (_crackStages[i] == null) continue;
                    // Stage 0 shows at <75%, stage 1 at <50%, stage 2 at <25%
                    float threshold = 1f - ((i + 1f) / (stageCount + 1f));
                    _crackStages[i].SetActive(ratio < threshold);
                }
            }

            if (evt.CurrentHp < _previousHp && _audioSource != null && _damageAlarmClip != null)
                _audioSource.PlayOneShot(_damageAlarmClip);

            _previousHp = evt.CurrentHp;
        }

        private void HandleShieldChanged(OnCoreShieldChanged evt)
        {
            bool hasShield = evt.MaxShield > 0f && evt.CurrentShield > 0f;

            if (_shieldBubble != null)
                _shieldBubble.SetActive(hasShield);

            if (_shieldMaterial != null && hasShield)
            {
                float ratio = evt.CurrentShield / evt.MaxShield;
                var color = _shieldMaterial.color;
                color.a = ratio * 0.5f;
                _shieldMaterial.color = color;
            }
        }

        private void HandleUpgraded(OnCoreUpgraded evt)
        {
            if (_glowLight != null)
                _glowLight.intensity = _baseGlowIntensity + (evt.NewTier * _glowIntensityPerTier);

            if (_glowParticles != null)
            {
                var emission = _glowParticles.emission;
                emission.rateOverTimeMultiplier = 1f + (evt.NewTier * 0.5f);
            }

            if (_coreModel != null)
                _coreModel.localScale = Vector3.one * (1f + evt.NewTier * _scalePerTier);
        }

        private void OnDestroy()
        {
            if (_eventBus == null) return;
            _eventBus.Unsubscribe(_onHealthChanged);
            _eventBus.Unsubscribe(_onShieldChanged);
            _eventBus.Unsubscribe(_onUpgraded);

            if (_shieldMaterial != null)
                Destroy(_shieldMaterial);
        }
    }
}
