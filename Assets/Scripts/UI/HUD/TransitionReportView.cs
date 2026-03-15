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
        [SerializeField] private float _displayDuration = 4f;

        private IEventBus _eventBus;
        private Action<OnTransitionReport> _onReport;
        private float _hideTimer;

        private void Start()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();

            _onReport = HandleReport;
            _eventBus.Subscribe(_onReport);

            if (_reportPanel != null)
                _reportPanel.SetActive(false);
        }

        private void Update()
        {
            if (_hideTimer > 0f)
            {
                _hideTimer -= Time.deltaTime;
                if (_hideTimer <= 0f && _reportPanel != null)
                    _reportPanel.SetActive(false);
            }
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

            _hideTimer = _displayDuration;
        }

        private void OnDestroy()
        {
            if (_eventBus == null) return;
            _eventBus.Unsubscribe(_onReport);
        }
    }
}
