using System;
using System.Collections;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using UnityEngine;
using UnityEngine.UI;

namespace RagdollRealms.UI.HUD
{
    public sealed class CoreHpView : MonoBehaviour
    {
        [Header("HP Bar")]
        [SerializeField] private Image _hpBarFill;
        [SerializeField] private Text _hpLabel;
        [SerializeField] private Text _percentLabel;

        [Header("Shield")]
        [SerializeField] private Image _shieldBarFill;
        [SerializeField] private GameObject _shieldIndicator;

        [Header("Colors")]
        [SerializeField] private Color _healthyColor = Color.green;
        [SerializeField] private Color _warningColor = Color.yellow;
        [SerializeField] private Color _criticalColor = Color.red;

        private IEventBus _eventBus;
        private Action<OnCoreHealthChanged> _onCoreHealthChanged;
        private Action<OnCoreShieldChanged> _onCoreShieldChanged;
        private float _previousHp;
        private Coroutine _pulseCoroutine;

        private void Start()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();

            _onCoreHealthChanged = HandleCoreHealthChanged;
            _onCoreShieldChanged = HandleCoreShieldChanged;

            _eventBus.Subscribe(_onCoreHealthChanged);
            _eventBus.Subscribe(_onCoreShieldChanged);
        }

        private void HandleCoreHealthChanged(OnCoreHealthChanged evt)
        {
            if (_hpBarFill != null)
            {
                float ratio = evt.MaxHp > 0f ? evt.CurrentHp / evt.MaxHp : 0f;
                _hpBarFill.fillAmount = ratio;

                if (ratio > 0.6f)
                    _hpBarFill.color = _healthyColor;
                else if (ratio > 0.3f)
                    _hpBarFill.color = Color.Lerp(_criticalColor, _warningColor, (ratio - 0.3f) / 0.3f);
                else
                    _hpBarFill.color = Color.Lerp(_criticalColor, _warningColor, ratio / 0.3f);
            }

            if (_hpLabel != null)
                _hpLabel.text = $"{Mathf.CeilToInt(evt.CurrentHp)}/{Mathf.CeilToInt(evt.MaxHp)}";

            if (_percentLabel != null)
            {
                float percent = evt.MaxHp > 0f ? (evt.CurrentHp / evt.MaxHp) * 100f : 0f;
                _percentLabel.text = $"{Mathf.RoundToInt(percent)}%";
            }

            if (_shieldBarFill != null)
            {
                float shieldRatio = evt.MaxShieldHp > 0f ? evt.ShieldHp / evt.MaxShieldHp : 0f;
                _shieldBarFill.fillAmount = shieldRatio;
            }

            if (_shieldIndicator != null)
                _shieldIndicator.SetActive(evt.MaxShieldHp > 0f && evt.ShieldHp > 0f);

            if (evt.CurrentHp < _previousHp && _hpBarFill != null)
            {
                _hpBarFill.transform.localScale = Vector3.one * 1.1f;
                if (_pulseCoroutine != null) StopCoroutine(_pulseCoroutine);
                _pulseCoroutine = StartCoroutine(PulseBack());
            }

            _previousHp = evt.CurrentHp;
        }

        private IEnumerator PulseBack()
        {
            yield return new WaitForSeconds(0.1f);
            if (_hpBarFill != null)
                _hpBarFill.transform.localScale = Vector3.one;
        }

        private void HandleCoreShieldChanged(OnCoreShieldChanged evt)
        {
            if (_shieldBarFill != null)
            {
                float ratio = evt.MaxShield > 0f ? evt.CurrentShield / evt.MaxShield : 0f;
                _shieldBarFill.fillAmount = ratio;
            }

            if (_shieldIndicator != null)
                _shieldIndicator.SetActive(evt.MaxShield > 0f && evt.CurrentShield > 0f);
        }

        private void OnDestroy()
        {
            if (_eventBus == null) return;
            _eventBus.Unsubscribe(_onCoreHealthChanged);
            _eventBus.Unsubscribe(_onCoreShieldChanged);
        }
    }
}
