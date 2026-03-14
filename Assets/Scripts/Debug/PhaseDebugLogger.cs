using System;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using UnityEngine;

namespace RagdollRealms.Debugging
{
    /// <summary>
    /// Temporary test helper. Logs phase events to Console.
    /// Delete after testing.
    /// </summary>
    public class PhaseDebugLogger : MonoBehaviour
    {
        private IEventBus _eventBus;
        private Action<OnPhaseChanged> _onPhaseChanged;
        private Action<OnPhaseTimerUpdate> _onTimerUpdate;
        private Action<OnGameSessionEnd> _onSessionEnd;
        private float _lastLogTime;

        private void Start()
        {
            if (!ServiceLocator.Instance.TryGet(out _eventBus))
            {
                UnityEngine.Debug.LogError("[PhaseDebugLogger] IEventBus not found.");
                return;
            }

            _onPhaseChanged = HandlePhaseChanged;
            _onTimerUpdate = HandleTimerUpdate;
            _onSessionEnd = HandleSessionEnd;

            _eventBus.Subscribe(_onPhaseChanged);
            _eventBus.Subscribe(_onTimerUpdate);
            _eventBus.Subscribe(_onSessionEnd);
        }

        private void HandlePhaseChanged(OnPhaseChanged evt)
        {
            UnityEngine.Debug.Log($"[Phase] {evt.PreviousPhase} --> {evt.NewPhase}");
        }

        private void HandleTimerUpdate(OnPhaseTimerUpdate evt)
        {
            // Log once per second to avoid console spam
            if (Time.time - _lastLogTime < 1f) return;
            _lastLogTime = Time.time;

            UnityEngine.Debug.Log($"[Phase] {evt.Phase} timer: {evt.RemainingTime:F0}s / {evt.TotalTime:F0}s");
        }

        private void HandleSessionEnd(OnGameSessionEnd evt)
        {
            var result = evt.IsVictory ? "VICTORY" : "GAME OVER";
            UnityEngine.Debug.Log($"[Phase] {result} — Waves: {evt.WavesCompleted}, Kills: {evt.EnemiesKilled}, Time: {evt.TimePlayed:F1}s");
        }

        private void OnDestroy()
        {
            if (_eventBus == null) return;
            _eventBus.Unsubscribe(_onPhaseChanged);
            _eventBus.Unsubscribe(_onTimerUpdate);
            _eventBus.Unsubscribe(_onSessionEnd);
        }
    }
}
