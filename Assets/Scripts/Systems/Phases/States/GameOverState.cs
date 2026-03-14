using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using RagdollRealms.Core.StateMachine;
using UnityEngine;

namespace RagdollRealms.Systems.Phases.States
{
    public class GameOverState : IState
    {
        private readonly PhaseManager _manager;
        private readonly IEventBus _eventBus;

        public GameOverState(PhaseManager manager, IEventBus eventBus)
        {
            _manager = manager;
            _eventBus = eventBus;
        }

        public void Enter()
        {
            _eventBus.Publish(new OnPhaseChanged(_manager.PreviousPhase, PhaseType.GameOver));
            _eventBus.Publish(new OnGameSessionEnd(
                false,
                _manager.CurrentWaveNumber,
                _manager.EnemiesKilled,
                Time.time - _manager.GameStartTime));

            Debug.Log("[GameOverState] Game Over.");
        }

        public void Update() { }
        public void Exit() { }
    }
}
