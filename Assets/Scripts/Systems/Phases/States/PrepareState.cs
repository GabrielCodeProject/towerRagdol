using System.Collections.Generic;
using RagdollRealms.Content;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using RagdollRealms.Core.StateMachine;
using UnityEngine;

namespace RagdollRealms.Systems.Phases.States
{
    public class PrepareState : IState
    {
        private readonly PhaseManager _manager;
        private readonly IEventBus _eventBus;
        private readonly PhaseConfigDefinition _config;
        private float _timeRemaining;
        private int _nextWarningIndex;
        private int _lastPublishedSecond;

        public PrepareState(PhaseManager manager, IEventBus eventBus, PhaseConfigDefinition config)
        {
            _manager = manager;
            _eventBus = eventBus;
            _config = config;
        }

        public void Enter()
        {
            _timeRemaining = _config.PrepareTimerDuration;
            _nextWarningIndex = 0;
            _lastPublishedSecond = -1;
            _manager.PrepareTimeRemaining = _timeRemaining;

            _eventBus.Publish(new OnPhaseChanged(_manager.PreviousPhase, PhaseType.Prepare));
        }

        public void Update()
        {
            _timeRemaining -= Time.deltaTime;
            _manager.PrepareTimeRemaining = Mathf.Max(0f, _timeRemaining);

            // Publish timer update only when the integer second changes
            int currentSecond = Mathf.CeilToInt(_timeRemaining);
            if (currentSecond != _lastPublishedSecond)
            {
                _lastPublishedSecond = currentSecond;
                _eventBus.Publish(new OnPhaseTimerUpdate(
                    PhaseType.Prepare, _timeRemaining, _config.PrepareTimerDuration));
            }

            IReadOnlyList<float> thresholds = _config.PrepareWarningThresholds;
            if (_nextWarningIndex < thresholds.Count
                && _timeRemaining <= thresholds[_nextWarningIndex])
            {
                _nextWarningIndex++;
            }

            if (_timeRemaining <= 0f)
            {
                _manager.PreviousPhase = PhaseType.Prepare;
                _manager.TransitionTo<DefendState>();
            }
        }

        public void Exit() { }
    }
}
