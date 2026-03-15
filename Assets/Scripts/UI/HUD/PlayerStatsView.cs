using System;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using UnityEngine;
using UnityEngine.UI;

namespace RagdollRealms.UI.HUD
{
    public class PlayerStatsView : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private Image _healthBarFill;
        [SerializeField] private Text _healthLabel;

        [Header("Mana")]
        [SerializeField] private Image _manaBarFill;
        [SerializeField] private Text _manaLabel;

        [Header("Level & XP")]
        [SerializeField] private Text _levelLabel;
        [SerializeField] private Image _xpBarFill;

        private IEventBus _eventBus;
        private Action<OnPlayerHealthChanged> _onHealthChanged;
        private Action<OnPlayerManaChanged> _onManaChanged;
        private Action<OnPlayerXpChanged> _onXpChanged;

        private void Start()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();

            _onHealthChanged = HandleHealthChanged;
            _onManaChanged = HandleManaChanged;
            _onXpChanged = HandleXpChanged;

            _eventBus.Subscribe(_onHealthChanged);
            _eventBus.Subscribe(_onManaChanged);
            _eventBus.Subscribe(_onXpChanged);
        }

        private void HandleHealthChanged(OnPlayerHealthChanged evt)
        {
            if (_healthBarFill != null)
                _healthBarFill.fillAmount = evt.Max > 0f ? Mathf.Clamp01(evt.Current / evt.Max) : 0f;

            if (_healthLabel != null)
                _healthLabel.text = $"{Mathf.CeilToInt(evt.Current)}/{Mathf.CeilToInt(evt.Max)}";
        }

        private void HandleManaChanged(OnPlayerManaChanged evt)
        {
            if (_manaBarFill != null)
                _manaBarFill.fillAmount = evt.Max > 0f ? Mathf.Clamp01(evt.Current / evt.Max) : 0f;

            if (_manaLabel != null)
                _manaLabel.text = $"{Mathf.CeilToInt(evt.Current)}/{Mathf.CeilToInt(evt.Max)}";
        }

        private void HandleXpChanged(OnPlayerXpChanged evt)
        {
            if (_levelLabel != null)
                _levelLabel.text = $"Lv.{evt.Level}";

            if (_xpBarFill != null)
                _xpBarFill.fillAmount = evt.XpToNextLevel > 0f ? Mathf.Clamp01(evt.Xp / evt.XpToNextLevel) : 0f;
        }

        private void OnDestroy()
        {
            if (_eventBus == null) return;
            _eventBus.Unsubscribe(_onHealthChanged);
            _eventBus.Unsubscribe(_onManaChanged);
            _eventBus.Unsubscribe(_onXpChanged);
        }
    }
}
