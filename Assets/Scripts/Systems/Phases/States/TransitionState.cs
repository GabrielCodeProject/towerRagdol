using RagdollRealms.Content;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using RagdollRealms.Core.StateMachine;
using UnityEngine;

namespace RagdollRealms.Systems.Phases.States
{
    public class TransitionState : IState
    {
        private readonly PhaseManager _manager;
        private readonly IEventBus _eventBus;
        private readonly PhaseConfigDefinition _config;
        private float _timeRemaining;

        public TransitionState(PhaseManager manager, IEventBus eventBus, PhaseConfigDefinition config)
        {
            _manager = manager;
            _eventBus = eventBus;
            _config = config;
        }

        public void Enter()
        {
            _timeRemaining = _config.TransitionDuration + _config.TransitionReportDuration;
            _manager.CurrentWaveNumber++;

            _eventBus.Publish(new OnPhaseChanged(_manager.PreviousPhase, PhaseType.Transition));
            _eventBus.Publish(new OnTransitionReport(
                _manager.CurrentWaveNumber,
                _manager.EnemiesKilled,
                _manager.StructuresDamaged));
        }

        public void Update()
        {
            _timeRemaining -= Time.deltaTime;

            if (_timeRemaining <= 0f)
            {
                if (_config.MaxWaves > 0 && _manager.CurrentWaveNumber >= _config.MaxWaves)
                {
                    _manager.TransitionTo<VictoryState>();
                    return;
                }

                int nextWave = _manager.CurrentWaveNumber + 1;
                _eventBus.Publish(new OnDifficultyEscalation(nextWave));

                if (_config.BossWaveInterval > 0
                    && _manager.CurrentWaveNumber % _config.BossWaveInterval == 0)
                {
                    _manager.TransitionTo<BossWaveState>();
                }
                else
                {
                    _manager.TransitionTo<PrepareState>();
                }
            }
        }

        public void Exit() { }
    }
}
