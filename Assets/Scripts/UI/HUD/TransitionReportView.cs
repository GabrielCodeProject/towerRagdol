using System;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using UnityEngine.UI;
using UnityEngine;

namespace RagdollRealms.UI.HUD
{
    public class TransitionReportView : MonoBehaviour
    {
        [SerializeField] private GameObject _reportPanel;
        [SerializeField] private Text _reportLabel;

        private IEventBus _eventBus;
        private Action<OnTransitionReport> _onReport;
        private Action<OnPhaseChanged> _onPhaseChanged;

        private void Start()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();

            _onReport = HandleReport;
            _onPhaseChanged = HandlePhaseChanged;
            _eventBus.Subscribe(_onReport);
            _eventBus.Subscribe(_onPhaseChanged);

            if (_reportPanel != null)
                _reportPanel.SetActive(false);
        }

        private void HandleReport(OnTransitionReport evt)
        {
            if (_reportPanel != null)
                _reportPanel.SetActive(true);

            if (_reportLabel != null)
            {
                _reportLabel.text =
                    $"Wave {evt.WaveNumber} Survived!\n" +
                    $"Enemies Killed: {evt.EnemiesKilled}\n" +
                    $"Structures Hit: {evt.StructuresDamaged}";
            }
        }

        private void HandlePhaseChanged(OnPhaseChanged evt)
        {
            if (evt.NewPhase != PhaseType.Transition && _reportPanel != null)
                _reportPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_eventBus == null) return;
            _eventBus.Unsubscribe(_onReport);
            _eventBus.Unsubscribe(_onPhaseChanged);
        }
    }
}
