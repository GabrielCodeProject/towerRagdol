using System.Collections;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollRealms.Debugging
{
    /// <summary>
    /// Temporary test helper. Calls StartGame() after one frame.
    /// Keys during play:
    ///   F5 = End defend wave (publish OnWaveEnd)
    ///   F6 = Simulate core destroyed (publish OnCoreHit with 0 HP)
    /// Delete after testing.
    /// </summary>
    public class PhaseTestRunner : MonoBehaviour
    {
        private IPhaseManager _phaseManager;
        private IEventBus _eventBus;
        private bool _ready;

        private IEnumerator Start()
        {
            yield return null;

            if (!ServiceLocator.Instance.TryGet(out _phaseManager))
            {
                Debug.LogError("[PhaseTestRunner] IPhaseManager not found.");
                yield break;
            }

            if (!ServiceLocator.Instance.TryGet(out _eventBus))
            {
                Debug.LogError("[PhaseTestRunner] IEventBus not found.");
                yield break;
            }

            Debug.Log("[PhaseTestRunner] Starting game...");
            _phaseManager.StartGame();
            _ready = true;
            Debug.Log("[PhaseTestRunner] Ready — F5: end wave, F6: destroy core");
        }

        private void Update()
        {
            if (!_ready) return;

            var kb = Keyboard.current;
            if (kb == null)
            {
                Debug.LogWarning("[PhaseTestRunner] No keyboard detected");
                return;
            }

            if (kb.f5Key.wasPressedThisFrame)
            {
                Debug.Log($"[PhaseTestRunner] F5 detected — current phase: {_phaseManager.CurrentPhase}");
                if (_phaseManager.IsInPhase(PhaseType.Defend))
                {
                    Debug.Log("[PhaseTestRunner] F5 → ending defend wave");
                    _eventBus.Publish(new OnWaveEnd(_phaseManager.CurrentWaveNumber, true));
                }
                else
                {
                    Debug.LogWarning($"[PhaseTestRunner] F5 ignored — not in Defend phase (current: {_phaseManager.CurrentPhase})");
                }
            }

            if (kb.f6Key.wasPressedThisFrame)
            {
                Debug.Log($"[PhaseTestRunner] F6 detected — current phase: {_phaseManager.CurrentPhase}");
                Debug.Log("[PhaseTestRunner] F6 → simulating core destroyed");
                _eventBus.Publish(new OnCoreHit(9999f, 0f, -1));
            }
        }
    }
}
