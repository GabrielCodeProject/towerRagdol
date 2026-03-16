using System;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using RagdollRealms.Core.StateMachine;

namespace RagdollRealms.Systems.Phases.States
{
    public class BossWaveState : IState
    {
        private readonly PhaseManager _manager;
        private readonly IEventBus _eventBus;
        private Action<OnBossDefeated> _onBossDefeated;
        private Action<OnCoreHit> _onCoreHit;
        private bool _transitioned;

        public BossWaveState(PhaseManager manager, IEventBus eventBus)
        {
            _manager = manager;
            _eventBus = eventBus;
        }

        public void Enter()
        {
            _transitioned = false;
            _onBossDefeated = HandleBossDefeated;
            _onCoreHit = HandleCoreHit;
            _eventBus.Subscribe(_onBossDefeated);
            _eventBus.Subscribe(_onCoreHit);

            _eventBus.Publish(new OnPhaseChanged(_manager.PreviousPhase, PhaseType.BossWave));
        }

        public void Update() { }

        public void Exit()
        {
            _eventBus.Unsubscribe(_onBossDefeated);
            _eventBus.Unsubscribe(_onCoreHit);
        }

        private void HandleBossDefeated(OnBossDefeated evt)
        {
            if (_transitioned) return;
            _transitioned = true;
            _manager.TransitionTo<VictoryState>();
        }

        private void HandleCoreHit(OnCoreHit evt)
        {
            if (_transitioned) return;
            _manager.CheckCoreDestroyed(evt);
        }
    }
}
