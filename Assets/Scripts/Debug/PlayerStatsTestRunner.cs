using System.Collections;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollRealms.Debugging
{
    /// <summary>
    /// Temporary test helper for player stats UI.
    /// Keys during play:
    ///   F7 = Take 10 damage
    ///   F8 = Spend 15 mana
    ///   F9 = Gain 50 XP
    ///   F10 = Cast spell (cycle cooldown on slot 0)
    /// Delete after testing.
    /// </summary>
    public class PlayerStatsTestRunner : MonoBehaviour
    {
        private IEventBus _eventBus;
        private bool _ready;

        private float _currentHp = 100f;
        private float _maxHp = 100f;
        private float _currentMana = 50f;
        private float _maxMana = 50f;
        private float _currentXp;
        private float _xpToNext = 200f;
        private int _level = 1;
        private float _spellCooldown;
        private float _spellCooldownTotal = 3f;

        private IEnumerator Start()
        {
            yield return null;

            if (!ServiceLocator.Instance.TryGet(out _eventBus))
            {
                Debug.LogError("[PlayerStatsTestRunner] IEventBus not found.");
                yield break;
            }

            _eventBus.Publish(new OnPlayerHealthChanged(0, _currentHp, _maxHp));
            _eventBus.Publish(new OnPlayerManaChanged(0, _currentMana, _maxMana));
            _eventBus.Publish(new OnPlayerXpChanged(0, _level, _currentXp, _xpToNext));
            _eventBus.Publish(new OnSpellSlotUpdated(0, 0, "Fireball", 0f, _spellCooldownTotal, 15f, true));
            _eventBus.Publish(new OnSpellSlotUpdated(0, 1, "Heal", 0f, 5f, 25f, true));
            _eventBus.Publish(new OnSpellSlotUpdated(0, 2, "Shield", 0f, 8f, 20f, true));
            _eventBus.Publish(new OnSpellSlotUpdated(0, 3, "Bolt", 0f, 2f, 10f, true));

            _ready = true;
            Debug.Log("[StatsTest] Ready — F7: damage, F8: mana, F9: xp, F10: cast spell");
        }

        private void Update()
        {
            if (!_ready) return;

            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb.f7Key.wasPressedThisFrame)
            {
                _currentHp = Mathf.Max(0f, _currentHp - 10f);
                _eventBus.Publish(new OnPlayerHealthChanged(0, _currentHp, _maxHp));
                Debug.Log($"[StatsTest] F7 → HP: {_currentHp}/{_maxHp}");

                if (_currentHp <= 0f)
                {
                    _eventBus.Publish(new OnCoreHit(9999f, 0f, -1));
                    Debug.Log("[StatsTest] HP reached 0 → triggered core destroyed");
                }
            }

            if (kb.f8Key.wasPressedThisFrame)
            {
                _currentMana = Mathf.Max(0f, _currentMana - 15f);
                _eventBus.Publish(new OnPlayerManaChanged(0, _currentMana, _maxMana));
                Debug.Log($"[StatsTest] F8 → Mana: {_currentMana}/{_maxMana}");
            }

            if (kb.f9Key.wasPressedThisFrame)
            {
                _currentXp += 50f;
                if (_currentXp >= _xpToNext)
                {
                    _currentXp -= _xpToNext;
                    _level++;
                    _xpToNext = _xpToNext * 1.5f;
                }
                _eventBus.Publish(new OnPlayerXpChanged(0, _level, _currentXp, _xpToNext));
                Debug.Log($"[StatsTest] F9 → Lv.{_level} XP: {_currentXp}/{_xpToNext}");
            }

            if (kb.f10Key.wasPressedThisFrame && _spellCooldown <= 0f)
            {
                _spellCooldown = _spellCooldownTotal;
                _currentMana = Mathf.Max(0f, _currentMana - 15f);
                _eventBus.Publish(new OnPlayerManaChanged(0, _currentMana, _maxMana));
                Debug.Log("[StatsTest] F10 → Cast Fireball!");
            }

            if (_spellCooldown > 0f)
            {
                _spellCooldown -= Time.deltaTime;
                bool isReady = _spellCooldown <= 0f;
                if (isReady) _spellCooldown = 0f;
                _eventBus.Publish(new OnSpellSlotUpdated(0, 0, "Fireball",
                    _spellCooldown, _spellCooldownTotal, 15f, isReady));
            }
        }
    }
}
