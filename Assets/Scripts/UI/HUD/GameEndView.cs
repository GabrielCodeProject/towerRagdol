using System;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using UnityEngine.UI;
using UnityEngine;

namespace RagdollRealms.UI.HUD
{
    public class GameEndView : MonoBehaviour
    {
        [SerializeField] private GameObject _endPanel;
        [SerializeField] private Text _titleLabel;
        [SerializeField] private Text _statsLabel;
        [SerializeField] private Text _celebrationLabel;

        private IEventBus _eventBus;
        private Action<OnGameSessionEnd> _onSessionEnd;
        private Action<OnVictoryCelebration> _onCelebration;

        private void Start()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();

            _onSessionEnd = HandleSessionEnd;
            _onCelebration = HandleCelebration;

            _eventBus.Subscribe(_onSessionEnd);
            _eventBus.Subscribe(_onCelebration);

            if (_endPanel != null)
                _endPanel.SetActive(false);
        }

        private void HandleSessionEnd(OnGameSessionEnd evt)
        {
            if (_endPanel != null)
                _endPanel.SetActive(true);

            if (_titleLabel != null)
                _titleLabel.text = evt.IsVictory ? "VICTORY!" : "GAME OVER";

            if (_titleLabel != null)
                _titleLabel.color = evt.IsVictory ? Color.yellow : Color.red;

            if (_statsLabel != null)
            {
                int minutes = Mathf.FloorToInt(evt.TimePlayed / 60f);
                int seconds = Mathf.FloorToInt(evt.TimePlayed % 60f);
                _statsLabel.text =
                    $"Waves Survived: {evt.WavesCompleted}\n" +
                    $"Enemies Killed: {evt.EnemiesKilled}\n" +
                    $"Bosses Defeated: {evt.BossesDefeated}\n" +
                    $"Time: {minutes:00}:{seconds:00}";
            }
        }

        private void HandleCelebration(OnVictoryCelebration evt)
        {
            if (_celebrationLabel != null)
            {
                _celebrationLabel.gameObject.SetActive(true);
                _celebrationLabel.text = evt.EndlessModeUnlocked
                    ? "Endless Mode Unlocked!"
                    : "";
            }
        }

        private void OnDestroy()
        {
            if (_eventBus == null) return;
            _eventBus.Unsubscribe(_onSessionEnd);
            _eventBus.Unsubscribe(_onCelebration);
        }
    }
}
