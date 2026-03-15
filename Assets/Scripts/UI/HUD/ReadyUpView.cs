using System;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using UnityEngine;
using UnityEngine.UI;

namespace RagdollRealms.UI.HUD
{
    public class ReadyUpView : MonoBehaviour
    {
        [SerializeField] private GameObject _readyPanel;
        [SerializeField] private Button _readyButton;
        [SerializeField] private Text _readyCountLabel;
        [SerializeField] private Text _buttonLabel;
        [SerializeField] private int _localPlayerId;

        private IEventBus _eventBus;
        private IPhaseManager _phaseManager;
        private Action<OnPhaseChanged> _onPhaseChanged;
        private Action<OnPlayerReadyChanged> _onReadyChanged;
        private bool _isReady;

        private void Start()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();
            _phaseManager = ServiceLocator.Instance.Get<IPhaseManager>();

            _onPhaseChanged = HandlePhaseChanged;
            _onReadyChanged = HandleReadyChanged;

            _eventBus.Subscribe(_onPhaseChanged);
            _eventBus.Subscribe(_onReadyChanged);

            if (_readyButton != null)
                _readyButton.onClick.AddListener(OnReadyClicked);

            if (_readyPanel != null)
                _readyPanel.SetActive(false);
        }

        private void OnReadyClicked()
        {
            if (_isReady) return;
            _isReady = true;
            _phaseManager.PlayerReady(_localPlayerId);

            if (_buttonLabel != null)
                _buttonLabel.text = "READY!";

            if (_readyButton != null)
                _readyButton.interactable = false;
        }

        private void HandlePhaseChanged(OnPhaseChanged evt)
        {
            bool showReady = evt.NewPhase == PhaseType.Prepare;

            if (_readyPanel != null)
                _readyPanel.SetActive(showReady);

            if (showReady)
            {
                _isReady = false;

                if (_buttonLabel != null)
                    _buttonLabel.text = "READY?";

                if (_readyButton != null)
                    _readyButton.interactable = true;

                if (_readyCountLabel != null)
                    _readyCountLabel.text = "0/1 Ready";
            }
        }

        private void HandleReadyChanged(OnPlayerReadyChanged evt)
        {
            if (_readyCountLabel != null)
                _readyCountLabel.text = $"{evt.ReadyCount}/{evt.TotalPlayers} Ready";
        }

        private void OnDestroy()
        {
            if (_readyButton != null)
                _readyButton.onClick.RemoveListener(OnReadyClicked);

            if (_eventBus == null) return;
            _eventBus.Unsubscribe(_onPhaseChanged);
            _eventBus.Unsubscribe(_onReadyChanged);
        }
    }
}
