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
        private Action<OnBossDefeated> _onBossDefeated;
        private Action<OnCoreHit> _onCoreHitTracker;
        private Action<OnPlayerConnected> _onPlayerConnected;
        private Action<OnPlayerDisconnected> _onPlayerDisconnected;
        private Action<OnPlayerReadyRequested> _onPlayerReadyRequested;

        public PhaseType CurrentPhase { get; private set; }
        public PhaseType PreviousPhase { get; private set; }
        public float PrepareTimeRemaining { get; internal set; }
        public int CurrentWaveNumber { get; internal set; }
        public int EnemiesKilled { get; private set; }
        public int BossesDefeated { get; private set; }
        public int StructuresDamaged { get; private set; }
        public float GameStartTime { get; private set; }

        internal PhaseConfigDefinition Config => _config;

        private void Awake()
        {
            ServiceLocator.Instance.Register<IPhaseManager>(this);
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
            _onBossDefeated = HandleBossDefeated;
            _onCoreHitTracker = HandleCoreHitTracker;
            _onPlayerConnected = HandlePlayerConnected;
            _onPlayerDisconnected = HandlePlayerDisconnected;
            _onPlayerReadyRequested = HandlePlayerReadyRequested;

            _eventBus.Subscribe(_onEnemyKilled);
            _eventBus.Subscribe(_onBossDefeated);
            _eventBus.Subscribe(_onCoreHitTracker);
            _eventBus.Subscribe(_onPlayerConnected);
            _eventBus.Subscribe(_onPlayerDisconnected);
            _eventBus.Subscribe(_onPlayerReadyRequested);
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

            GameStartTime = Time.time;
            CurrentWaveNumber = 0;
            EnemiesKilled = 0;
            BossesDefeated = 0;
            StructuresDamaged = 0;
            PreviousPhase = PhaseType.Prepare;
            CurrentPhase = PhaseType.Prepare;

            _stateMachine.SetState<PrepareState>();
        }

        public void PlayerReady(int playerId)
        {
            if (!IsInPhase(PhaseType.Prepare)) return;

            _readyPlayers.Add(playerId);

            _eventBus.Publish(new OnPlayerReadyChanged(
                playerId, true, _readyPlayers.Count, _trackedPlayerCount));

            int requiredCount = Mathf.CeilToInt(_trackedPlayerCount * _config.ReadyUpMajorityThreshold);
            if (_readyPlayers.Count >= requiredCount)
            {
                _readyPlayers.Clear();
                TransitionTo<DefendState>();
            }
        }

        internal void ClearReadyPlayers()
        {
            _readyPlayers.Clear();
        }

        public bool IsInPhase(PhaseType phase)
        {
            return CurrentPhase == phase;
        }

        internal void TransitionTo<T>() where T : IState
        {
            PreviousPhase = CurrentPhase;
            var phaseType = StateToPhaseType(typeof(T));
            CurrentPhase = phaseType;
            _stateMachine.SetState<T>();
        }

        internal void CheckCoreDestroyed(OnCoreHit evt)
        {
            if (evt.RemainingHp <= 0f)
            {
                TransitionTo<GameOverState>();
            }
        }

        private void HandleEnemyKilled(OnEnemyKilled evt)
        {
            EnemiesKilled++;
        }

        private void HandleBossDefeated(OnBossDefeated evt)
        {
            BossesDefeated++;
        }

        private void HandleCoreHitTracker(OnCoreHit evt)
        {
            StructuresDamaged++;
        }

        private void HandlePlayerReadyRequested(OnPlayerReadyRequested evt)
        {
            PlayerReady(evt.PlayerId);
        }

        private void HandlePlayerConnected(OnPlayerConnected evt)
        {
            _trackedPlayerCount++;
        }

        private void HandlePlayerDisconnected(OnPlayerDisconnected evt)
        {
            _trackedPlayerCount = Mathf.Max(1, _trackedPlayerCount - 1);
            _readyPlayers.Remove(evt.PlayerId);
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
                _eventBus.Unsubscribe(_onBossDefeated);
                _eventBus.Unsubscribe(_onCoreHitTracker);
                _eventBus.Unsubscribe(_onPlayerConnected);
                _eventBus.Unsubscribe(_onPlayerDisconnected);
                _eventBus.Unsubscribe(_onPlayerReadyRequested);
            }

            if (ServiceLocator.Instance == null) return;
            ServiceLocator.Instance.Unregister<IPhaseManager>();
            ServiceLocator.Instance.Unregister<IPhaseGate>();
        }
    }
}
