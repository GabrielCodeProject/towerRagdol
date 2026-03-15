using System;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using UnityEngine;
using UnityEngine.UI;

namespace RagdollRealms.UI.HUD
{
    public class SpellToolbarView : MonoBehaviour
    {
        [Serializable]
        public class SpellSlotUI
        {
            public Image Icon;
            public Image CooldownFill;
            public Text NameLabel;
            public Text ManaCostLabel;
            public GameObject ReadyGlow;
        }

        [SerializeField] private SpellSlotUI[] _slots = new SpellSlotUI[4];

        private IEventBus _eventBus;
        private Action<OnSpellSlotUpdated> _onSlotUpdated;

        private void Start()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();

            _onSlotUpdated = HandleSlotUpdated;
            _eventBus.Subscribe(_onSlotUpdated);

            for (int i = 0; i < _slots.Length; i++)
                SetSlotEmpty(i);
        }

        private void HandleSlotUpdated(OnSpellSlotUpdated evt)
        {
            if (evt.SlotIndex < 0 || evt.SlotIndex >= _slots.Length) return;

            var slot = _slots[evt.SlotIndex];
            if (slot == null) return;

            float fill = evt.CooldownTotal > 0f
                ? Mathf.Clamp01(evt.CooldownRemaining / evt.CooldownTotal)
                : 0f;

            ApplySlotUI(slot, evt.SpellName ?? "", $"{Mathf.CeilToInt(evt.ManaCost)}", fill, evt.IsReady);
        }

        private void SetSlotEmpty(int index)
        {
            if (index < 0 || index >= _slots.Length) return;

            var slot = _slots[index];
            if (slot == null) return;

            ApplySlotUI(slot, "", "", 0f, false);
        }

        private void ApplySlotUI(SpellSlotUI slot, string name, string manaCost, float cooldownFill, bool isReady)
        {
            if (slot.NameLabel != null)
                slot.NameLabel.text = name;

            if (slot.ManaCostLabel != null)
                slot.ManaCostLabel.text = manaCost;

            if (slot.CooldownFill != null)
                slot.CooldownFill.fillAmount = cooldownFill;

            if (slot.ReadyGlow != null)
                slot.ReadyGlow.SetActive(isReady);
        }

        private void OnDestroy()
        {
            if (_eventBus == null) return;
            _eventBus.Unsubscribe(_onSlotUpdated);
        }
    }
}
