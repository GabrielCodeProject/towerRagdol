using System;
using System.Collections;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using UnityEngine.UI;
using UnityEngine;

namespace RagdollRealms.UI.HUD
{
    public class WaveAnnouncementView : MonoBehaviour
    {
        [SerializeField] private GameObject _announcementPanel;
        [SerializeField] private Text _waveLabel;
        [SerializeField] private Text _escalationLabel;
        [SerializeField] private float _displayDuration = 3f;

        private IEventBus _eventBus;
        private Action<OnWaveAnnouncement> _onAnnouncement;
        private Action<OnDifficultyEscalation> _onEscalation;
        private Coroutine _hideCoroutine;

        private void Start()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();

            _onAnnouncement = HandleAnnouncement;
            _onEscalation = HandleEscalation;

            _eventBus.Subscribe(_onAnnouncement);
            _eventBus.Subscribe(_onEscalation);

            if (_announcementPanel != null)
                _announcementPanel.SetActive(false);

            if (_escalationLabel != null)
                _escalationLabel.gameObject.SetActive(false);
        }

        private void HandleAnnouncement(OnWaveAnnouncement evt)
        {
            if (_announcementPanel != null)
                _announcementPanel.SetActive(true);

            if (_waveLabel != null)
                _waveLabel.text = $"WAVE {evt.WaveNumber}";

            RestartHideTimer();
        }

        private void HandleEscalation(OnDifficultyEscalation evt)
        {
            if (_escalationLabel != null)
            {
                _escalationLabel.gameObject.SetActive(true);
                _escalationLabel.text = $"Next: Wave {evt.NextWaveNumber}";
            }

            RestartHideTimer();
        }

        private void RestartHideTimer()
        {
            if (_hideCoroutine != null)
                StopCoroutine(_hideCoroutine);
            _hideCoroutine = StartCoroutine(HideAfterDelay());
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(_displayDuration);

            if (_announcementPanel != null)
                _announcementPanel.SetActive(false);
            if (_escalationLabel != null)
                _escalationLabel.gameObject.SetActive(false);

            _hideCoroutine = null;
        }

        private void OnDestroy()
        {
            if (_hideCoroutine != null)
                StopCoroutine(_hideCoroutine);

            if (_eventBus == null) return;
            _eventBus.Unsubscribe(_onAnnouncement);
            _eventBus.Unsubscribe(_onEscalation);
        }
    }
}
