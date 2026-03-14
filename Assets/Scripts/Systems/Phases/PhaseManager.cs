using System;
using System.Collections.Generic;
using RagdollRealms.Content;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using RagdollRealms.Core.StateMachine;
using RagdollRealms.Systems.Phases.States;
using UnityEngine;

namespace RagdollRealms.Systems.Phases
{
    public class PhaseManager : MonoBehaviour, IPhaseManager
    {
        [SerializeField] private PhaseConfigDefinition _config;

        private Core.StateMachine.StateMachine _stateMachine;
        private IEventBus _eventBus;
        private PhaseGate _phaseGate;
        private readonly HashSet<int> _readyPlayers = new();
        private int _trackedPlayerCount = 1;
        private Action<OnEnemyKilled> _onEnemyKilled;

        public PhaseType CurrentPhase { get; private set; }
        public PhaseType PreviousPhase { get; internal set; }
        public float PrepareTimeRemaining { get; internal set; }
        public int CurrentWaveNumber { get; internal set; }
        public int EnemiesKilled { get; private set; }
        public float GameStartTime { get; private set; }

        private void Awake()
        {
            ServiceLocator.Instance.Register<IPhaseManager>(this);
            Debug.Log("[PhaseManager] Registered IPhaseManager.");
        }

        private void Start()
        {
            if (_config == null)
            {
                Debug.LogError("[PhaseManager] _config is not assigned! Drag DefaultPhaseConfig into the Config field.");
                return;
            }

            _eventBus = ServiceLocator.Instance.Get<IEventBus>();

            _phaseGate = new PhaseGate(this, _config);
            ServiceLocator.Instance.Register<IPhaseGate>(_phaseGate);

            _stateMachine = new Core.StateMachine.StateMachine();
            _stateMachine.AddState(new PrepareState(this, _eventBus, _config));
            _stateMachine.AddState(new DefendState(this, _eventBus));
            _stateMachine.AddState(new TransitionState(this, _eventBus, _config));
            _stateMachine.AddState(new BossWaveState(this, _eventBus));
            _stateMachine.AddState(new GameOverState(this, _eventBus));
            _stateMachine.AddState(new VictoryState(this, _eventBus));

            _onEnemyKilled = HandleEnemyKilled;
            _eventBus.Subscribe(_onEnemyKilled);

            Debug.Log("[PhaseManager] Initialized with config: " + _config.name);
        }

        private void Update()
        {
            _stateMachine?.Update();
        }

        public void StartGame()
        {
            if (_stateMachine == null)
            {
                Debug.LogError("[PhaseManager] StartGame() called but state machine is not initialized.");
                return;
            }

            Debug.Log("[PhaseManager] StartGame() called.");
            GameStartTime = Time.time;
            CurrentWaveNumber = 0;
            EnemiesKilled = 0;
            PreviousPhase = PhaseType.Prepare;
            CurrentPhase = PhaseType.Prepare;

            _stateMachine.SetState<PrepareState>();
        }

        public void PlayerReady(int playerId)
        {
            _readyPlayers.Add(playerId);

            if (_readyPlayers.Count >= _trackedPlayerCount && IsInPhase(PhaseType.Prepare))
            {
                _readyPlayers.Clear();
                PreviousPhase = PhaseType.Prepare;
                TransitionTo<DefendState>();
            }
        }

        public bool IsInPhase(PhaseType phase)
        {
            return CurrentPhase == phase;
        }

        internal void TransitionTo<T>() where T : IState
        {
            var phaseType = StateToPhaseType(typeof(T));
            CurrentPhase = phaseType;
            _stateMachine.SetState<T>();
        }

        internal void CheckCoreDestroyed(OnCoreHit evt, PhaseType fromPhase)
        {
            if (evt.RemainingHp <= 0f)
            {
                PreviousPhase = fromPhase;
                TransitionTo<GameOverState>();
            }
        }

        private void HandleEnemyKilled(OnEnemyKilled evt)
        {
            EnemiesKilled++;
        }

        private static PhaseType StateToPhaseType(Type stateType)
        {
            if (stateType == typeof(PrepareState)) return PhaseType.Prepare;
            if (stateType == typeof(DefendState)) return PhaseType.Defend;
            if (stateType == typeof(TransitionState)) return PhaseType.Transition;
            if (stateType == typeof(BossWaveState)) return PhaseType.BossWave;
            if (stateType == typeof(GameOverState)) return PhaseType.GameOver;
            if (stateType == typeof(VictoryState)) return PhaseType.Victory;
            return PhaseType.Prepare;
        }

        private void OnDestroy()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe(_onEnemyKilled);
            }

            if (ServiceLocator.Instance == null) return;
            ServiceLocator.Instance.Unregister<IPhaseManager>();
            ServiceLocator.Instance.Unregister<IPhaseGate>();
        }
    }
}
