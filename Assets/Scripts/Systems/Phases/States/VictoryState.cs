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
            _eventBus.Publish(new OnPhaseChanged(_manager.PreviousPhase, PhaseType.Victory));
            _eventBus.Publish(new OnGameSessionEnd(
                true,
                _manager.CurrentWaveNumber,
                _manager.EnemiesKilled,
                Time.time - _manager.GameStartTime));

            Debug.Log("[VictoryState] Victory!");
        }

        public void Update() { }
        public void Exit() { }
    }
}
