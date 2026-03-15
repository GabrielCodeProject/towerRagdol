using UnityEngine;
using UnityEngine.InputSystem;
using RagdollRealms.Core;
using Debug = UnityEngine.Debug;

namespace RagdollRealms.Debugging
{
    /// <summary>
    /// Debug helper to test Core Base features at runtime.
    /// Press number keys to damage, heal, upgrade, etc.
    /// </summary>
    public sealed class CoreBaseTestRunner : MonoBehaviour
    {
        [Header("Test Controls")]
        [SerializeField] private float _damageAmount = 100f;
        [SerializeField] private float _healAmount = 50f;

        private ICoreManager _coreManager;

        private void Start()
        {
            if (!ServiceLocator.Instance.TryGet(out _coreManager))
            {
                Debug.LogWarning("[CoreBaseTestRunner] ICoreManager not found — waiting...");
            }
        }

        private void Update()
        {
            if (_coreManager == null)
            {
                ServiceLocator.Instance.TryGet(out _coreManager);
                return;
            }

            var kb = Keyboard.current;
            if (kb == null) return;

            // Require Left Shift to avoid conflicts with other key handlers
            if (!kb.leftShiftKey.isPressed) return;

            // [1] Damage Core
            if (kb.digit1Key.wasPressedThisFrame)
            {
                _coreManager.TakeDamage(_damageAmount, 0);
                Debug.Log($"[CoreTest] Damaged {_damageAmount} → HP: {_coreManager.CurrentHp}/{_coreManager.MaxHp}");
            }

            // [2] Heal Core
            if (kb.digit2Key.wasPressedThisFrame)
            {
                _coreManager.Heal(_healAmount);
                Debug.Log($"[CoreTest] Healed {_healAmount} → HP: {_coreManager.CurrentHp}/{_coreManager.MaxHp}");
            }

            // [3] Upgrade Core
            if (kb.digit3Key.wasPressedThisFrame)
            {
                bool success = _coreManager.UpgradeCore();
                Debug.Log($"[CoreTest] Upgrade {(success ? "SUCCESS" : "FAILED")} → Tier: {_coreManager.CurrentTier}, MaxHP: {_coreManager.MaxHp}, Shield: {_coreManager.MaxShieldHp}");
            }

            // [4] Recharge Shield
            if (kb.digit4Key.wasPressedThisFrame)
            {
                _coreManager.RechargeShield();
                Debug.Log($"[CoreTest] Shield recharged → {_coreManager.ShieldHp}/{_coreManager.MaxShieldHp}");
            }

            // [5] Heavy Damage (test shield + HP)
            if (kb.digit5Key.wasPressedThisFrame)
            {
                _coreManager.TakeDamage(300f, 0);
                Debug.Log($"[CoreTest] Heavy damage 300 → HP: {_coreManager.CurrentHp}/{_coreManager.MaxHp}, Shield: {_coreManager.ShieldHp}/{_coreManager.MaxShieldHp}");
            }
        }

        private void OnGUI()
        {
            if (_coreManager == null) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("<b>Core Base Test Controls</b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 14 });
            GUILayout.Label($"HP: {_coreManager.CurrentHp:F0}/{_coreManager.MaxHp:F0}");
            GUILayout.Label($"Shield: {_coreManager.ShieldHp:F0}/{_coreManager.MaxShieldHp:F0}");
            GUILayout.Label($"Tier: {_coreManager.CurrentTier} | Destroyed: {_coreManager.IsDestroyed}");
            GUILayout.Label($"Aura: R={_coreManager.HealingAuraRadius:F1} Rate={_coreManager.HealingAuraRate:F1}");
            GUILayout.Label($"Alarm Range: {_coreManager.AlarmRange:F1}");
            GUILayout.Space(5);
            GUILayout.Label("Hold LShift + [1] Damage  [2] Heal  [3] Upgrade");
            GUILayout.Label("Hold LShift + [4] Recharge Shield  [5] Heavy Damage");
            GUILayout.EndArea();
        }
    }
}
