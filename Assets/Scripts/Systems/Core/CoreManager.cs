using System;
using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using RagdollRealms.Content;

namespace RagdollRealms.Systems.CoreBase
{
    [DefaultExecutionOrder(-80)]
    public sealed class CoreManager : MonoBehaviour, ICoreManager
    {
        // State
        private float _currentHp;
        private float _maxHp;
        private float _shieldHp;
        private float _maxShieldHp;
        private int _currentTier;
        private bool _isDestroyed;
        private float _healingAuraRadius;
        private float _healingAuraRate;
        private float _alarmRange;

        // Services
        private IEventBus _eventBus;
        private ISaveManager _saveManager;
        private CoreDefinition _config;

        // Event delegates (for unsubscription)
        private Action<OnPhaseChanged> _onPhaseChanged;

        // ICoreManager properties
        public float CurrentHp => _currentHp;
        public float MaxHp => _maxHp;
        public float ShieldHp => _shieldHp;
        public float MaxShieldHp => _maxShieldHp;
        public int CurrentTier => _currentTier;
        public bool IsDestroyed => _isDestroyed;
        public float HealingAuraRadius => _healingAuraRadius;
        public float HealingAuraRate => _healingAuraRate;
        public float AlarmRange => _alarmRange;

        private void Awake()
        {
            ServiceLocator.Instance.Register<ICoreManager>(this);
        }

        private void Start()
        {
            ServiceLocator.Instance.TryGet(out _eventBus);
            if (!ServiceLocator.Instance.TryGet(out _saveManager))
                Debug.LogWarning("[CoreManager] ISaveManager not registered — save disabled");

            // Load config from registry
            var registry = ServiceLocator.Instance.Get<IContentRegistry<CoreDefinition>>();
            var allConfigs = registry.GetAll();
            if (allConfigs.Count > 0)
                _config = allConfigs[0];

            if (_config == null)
            {
                Debug.LogError("[CoreManager] No CoreDefinition found in registry!");
                return;
            }

            // Initialize state
            _maxHp = _config.BaseHp;
            _currentHp = _maxHp;
            _currentTier = 0;
            _shieldHp = 0f;
            _maxShieldHp = 0f;
            _healingAuraRadius = _config.HealingAuraRadius;
            _healingAuraRate = _config.HealingAuraRate;
            _alarmRange = _config.AlarmBaseRange;

            // Subscribe to events
            _onPhaseChanged = HandlePhaseChanged;
            _eventBus?.Subscribe(_onPhaseChanged);

            // Publish initial state for UI
            PublishHealthChanged();
        }

        public void TakeDamage(float damage, int attackerId)
        {
            if (_isDestroyed || damage <= 0f) return;

            float remainingDamage = damage;

            // Shield absorbs first
            if (_shieldHp > 0f)
            {
                float shieldAbsorbed = Mathf.Min(_shieldHp, remainingDamage);
                _shieldHp -= shieldAbsorbed;
                remainingDamage -= shieldAbsorbed;
                _eventBus?.Publish(new OnCoreShieldChanged(_shieldHp, _maxShieldHp));
            }

            // Remaining damage goes to HP
            if (remainingDamage > 0f)
            {
                _currentHp = Mathf.Max(0f, _currentHp - remainingDamage);
            }

            // Publish existing OnCoreHit event (SSOT — reuse, not duplicate)
            _eventBus?.Publish(new OnCoreHit(damage, _currentHp, attackerId));
            PublishHealthChanged();

            // Persist HP
            _saveManager?.SetCoreHp(_currentHp);

            // Check destruction
            if (_currentHp <= 0f)
            {
                _isDestroyed = true;
                _eventBus?.Publish(new OnCoreDestroyed());
            }
        }

        public void Heal(float amount)
        {
            if (_isDestroyed || amount <= 0f) return;
            _currentHp = Mathf.Min(_currentHp + amount, _maxHp);
            PublishHealthChanged();
        }

        public bool UpgradeCore()
        {
            if (_config == null || _currentTier >= _config.UpgradeTiers.Count || _config.UpgradeTiers.Count == 0)
                return false;

            // TODO: Material check not implemented — stub until inventory system exists
            Debug.LogWarning("[CoreManager] Material check stubbed — upgrade granted freely");

            _currentTier++;
            var tier = _config.UpgradeTiers[_currentTier - 1];

            // Recalculate stats from tier multipliers
            _maxHp = _config.BaseHp * tier.HpMultiplier;
            _currentHp = Mathf.Min(_currentHp, _maxHp);
            _maxShieldHp = tier.ShieldCapacity;
            _healingAuraRadius = _config.HealingAuraRadius * tier.AuraRadiusMultiplier;
            _healingAuraRate = _config.HealingAuraRate * tier.AuraRateMultiplier;
            _alarmRange = _config.AlarmBaseRange * tier.AlarmRangeMultiplier;

            _eventBus?.Publish(new OnCoreUpgraded(_currentTier, "core"));
            PublishHealthChanged();

            return true;
        }

        public void RechargeShield()
        {
            if (_maxShieldHp <= 0f) return;
            _shieldHp = _maxShieldHp;
            _eventBus?.Publish(new OnCoreShieldChanged(_shieldHp, _maxShieldHp));
            PublishHealthChanged();
        }

        private void HandlePhaseChanged(OnPhaseChanged evt)
        {
            // Recharge shield when entering Prepare phase
            if (evt.NewPhase == PhaseType.Prepare)
            {
                RechargeShield();
            }
        }

        private void PublishHealthChanged()
        {
            _eventBus?.Publish(new OnCoreHealthChanged(_currentHp, _maxHp, _shieldHp, _maxShieldHp));
        }

        private void OnDestroy()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe(_onPhaseChanged);
            }

            if (ServiceLocator.Instance == null) return;
            ServiceLocator.Instance.Unregister<ICoreManager>();
        }
    }
}
