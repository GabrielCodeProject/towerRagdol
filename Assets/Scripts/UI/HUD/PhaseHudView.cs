using System;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using UnityEngine.UI;
using UnityEngine;

namespace RagdollRealms.UI.HUD
{
    public class PhaseHudView : MonoBehaviour
    {
        [SerializeField] private Text _phaseLabel;
        [SerializeField] private Text _timerLabel;
        [SerializeField] private GameObject _warningIndicator;

        private IEventBus _eventBus;
        private Action<OnPhaseChanged> _onPhaseChanged;
        private Action<OnPhaseTimerUpdate> _onTimerUpdate;
        private Action<OnPrepareWarning> _onWarning;
        private float _warningFlashTimer;

        private void Start()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();

            _onPhaseChanged = HandlePhaseChanged;
            _onTimerUpdate = HandleTimerUpdate;
            _onWarning = HandleWarning;

            _eventBus.Subscribe(_onPhaseChanged);
            _eventBus.Subscribe(_onTimerUpdate);
            _eventBus.Subscribe(_onWarning);

            if (_warningIndicator != null)
                _warningIndicator.SetActive(false);

            if (_timerLabel != null)
                _timerLabel.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_warningFlashTimer > 0f)
            {
                _warningFlashTimer -= Time.deltaTime;
                if (_warningFlashTimer <= 0f && _warningIndicator != null)
                    _warningIndicator.SetActive(false);
            }
        }

        private void HandlePhaseChanged(OnPhaseChanged evt)
        {
            if (_phaseLabel != null)
                _phaseLabel.text = evt.NewPhase.ToString().ToUpper();

            bool showTimer = evt.NewPhase == PhaseType.Prepare;
            if (_timerLabel != null)
                _timerLabel.gameObject.SetActive(showTimer);
        }

        private void HandleTimerUpdate(OnPhaseTimerUpdate evt)
        {
            if (_timerLabel == null) return;

            int minutes = Mathf.FloorToInt(evt.RemainingTime / 60f);
            int seconds = Mathf.FloorToInt(evt.RemainingTime % 60f);
            _timerLabel.text = $"{minutes:00}:{seconds:00}";

            if (evt.RemainingTime <= 10f)
                _timerLabel.color = Color.red;
            else if (evt.RemainingTime <= 30f)
                _timerLabel.color = Color.yellow;
            else
                _timerLabel.color = Color.white;
        }

        private void HandleWarning(OnPrepareWarning evt)
        {
            if (_warningIndicator != null)
            {
                _warningIndicator.SetActive(true);
                _warningFlashTimer = 2f;
            }
        }

        private void OnDestroy()
        {
            if (_eventBus == null) return;
            _eventBus.Unsubscribe(_onPhaseChanged);
            _eventBus.Unsubscribe(_onTimerUpdate);
            _eventBus.Unsubscribe(_onWarning);
        }
    }
}
