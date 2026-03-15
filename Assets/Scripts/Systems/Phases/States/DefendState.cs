using System;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using RagdollRealms.Core.StateMachine;

namespace RagdollRealms.Systems.Phases.States
{
    public class DefendState : IState
    {
        private readonly PhaseManager _manager;
        private readonly IEventBus _eventBus;
        private Action<OnWaveEnd> _onWaveEnd;
        private Action<OnCoreHit> _onCoreHit;
        private bool _transitioned;

        public DefendState(PhaseManager manager, IEventBus eventBus)
        {
            _manager = manager;
            _eventBus = eventBus;
        }

        public void Enter()
        {
            _transitioned = false;
            _onWaveEnd = HandleWaveEnd;
            _onCoreHit = HandleCoreHit;
            _eventBus.Subscribe(_onWaveEnd);
            _eventBus.Subscribe(_onCoreHit);

            _eventBus.Publish(new OnPhaseChanged(_manager.PreviousPhase, PhaseType.Defend));

            int waveNumber = _manager.CurrentWaveNumber + 1;
            _eventBus.Publish(new OnWaveAnnouncement(waveNumber, 0));
            _eventBus.Publish(new OnWaveStart(waveNumber, 0));
        }

        public void Update() { }

        public void Exit()
        {
            _eventBus.Unsubscribe(_onWaveEnd);
            _eventBus.Unsubscribe(_onCoreHit);
        }

        private void HandleWaveEnd(OnWaveEnd evt)
        {
            if (_transitioned) return;
            _transitioned = true;
            _manager.PreviousPhase = PhaseType.Defend;
            _manager.TransitionTo<TransitionState>();
        }

        private void HandleCoreHit(OnCoreHit evt)
        {
            if (_transitioned) return;
            _manager.CheckCoreDestroyed(evt, PhaseType.Defend);
        }
    }
}
