using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using RagdollRealms.Core.StateMachine;
using UnityEngine;

namespace RagdollRealms.Systems.Phases.States
{
    public class VictoryState : IState
    {
        private readonly PhaseManager _manager;
        private readonly IEventBus _eventBus;

        public VictoryState(PhaseManager manager, IEventBus eventBus)
        {
            _manager = manager;
            _eventBus = eventBus;
        }

        public void Enter()
        {
            bool endlessModeUnlocked = _manager.Config.EndlessModeWaveThreshold <= 0
                || _manager.CurrentWaveNumber >= _manager.Config.EndlessModeWaveThreshold;

            _eventBus.Publish(new OnPhaseChanged(_manager.PreviousPhase, PhaseType.Victory));
            _eventBus.Publish(new OnVictoryCelebration(endlessModeUnlocked));
            _eventBus.Publish(new OnGameSessionEnd(
                true,
                _manager.CurrentWaveNumber,
                _manager.EnemiesKilled,
                _manager.BossesDefeated,
                Time.time - _manager.GameStartTime));
        }

        public void Update() { }
        public void Exit() { }
    }
}
