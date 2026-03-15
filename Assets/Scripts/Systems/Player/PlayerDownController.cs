using System;
using System.Collections.Generic;
using RagdollRealms.Content;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using UnityEngine;

namespace RagdollRealms.Systems.Player
{
    public class PlayerDownController : MonoBehaviour, IPlayerDownController
    {
        [SerializeField] private PhaseConfigDefinition _config;

        private IEventBus _eventBus;
        private readonly HashSet<int> _downedPlayers = new();
        private readonly Dictionary<int, ReviveState> _activeRevives = new();
        private readonly HashSet<int> _allPlayers = new();
        private float _respawnTimer;
        private bool _allDowned;

        private Action<OnPlayerDowned> _onPlayerDowned;
        private Action<OnPlayerRevived> _onPlayerRevived;
        private Action<OnPlayerConnected> _onPlayerConnected;
        private Action<OnPlayerDisconnected> _onPlayerDisconnected;
        private Action<OnPhaseChanged> _onPhaseChanged;

        private void Awake()
        {
            ServiceLocator.Instance.Register<IPlayerDownController>(this);
        }

        private void Start()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();

            _onPlayerDowned = HandlePlayerDowned;
            _onPlayerRevived = HandlePlayerRevived;
            _onPlayerConnected = HandlePlayerConnected;
            _onPlayerDisconnected = HandlePlayerDisconnected;
            _onPhaseChanged = HandlePhaseChanged;

            _eventBus.Subscribe(_onPlayerDowned);
            _eventBus.Subscribe(_onPlayerRevived);
            _eventBus.Subscribe(_onPlayerConnected);
            _eventBus.Subscribe(_onPlayerDisconnected);
            _eventBus.Subscribe(_onPhaseChanged);
        }

        private void Update()
        {
            UpdateRevives();
            UpdateRespawnTimer();
        }

        public bool IsPlayerDowned(int playerId)
        {
            return _downedPlayers.Contains(playerId);
        }

        public IReadOnlyCollection<int> GetDownedPlayers()
        {
            return _downedPlayers;
        }

        public void StartRevive(int reviverId, int downedPlayerId)
        {
            if (!_downedPlayers.Contains(downedPlayerId)) return;
            if (_activeRevives.ContainsKey(downedPlayerId)) return;

            _activeRevives[downedPlayerId] = new ReviveState(reviverId, 0f);
            _eventBus.Publish(new OnPlayerReviveStarted(reviverId, downedPlayerId));
        }

        public void CancelRevive(int downedPlayerId)
        {
            _activeRevives.Remove(downedPlayerId);
        }

        public bool IsReviveInProgress(int downedPlayerId)
        {
            return _activeRevives.ContainsKey(downedPlayerId);
        }

        private void UpdateRevives()
        {
            if (_activeRevives.Count == 0) return;

            float reviveDuration = _config != null ? _config.ReviveDuration : 3f;
            var completed = new List<int>();

            foreach (var kvp in _activeRevives)
            {
                int downedId = kvp.Key;
                var state = kvp.Value;
                state.Progress += Time.deltaTime / reviveDuration;

                _eventBus.Publish(new OnPlayerReviveProgress(downedId, Mathf.Clamp01(state.Progress)));

                if (state.Progress >= 1f)
                {
                    completed.Add(downedId);
                    _eventBus.Publish(new OnPlayerRevived(downedId, state.ReviverId));
                }
                else
                {
                    _activeRevives[downedId] = state;
                }
            }

            foreach (int id in completed)
            {
                _activeRevives.Remove(id);
            }
        }

        private void UpdateRespawnTimer()
        {
            if (!_allDowned) return;

            _respawnTimer -= Time.deltaTime;
            if (_respawnTimer <= 0f)
            {
                _allDowned = false;
                var toRevive = new List<int>(_downedPlayers);
                foreach (int playerId in toRevive)
                {
                    _eventBus.Publish(new OnPlayerRevived(playerId, -1));
                }
            }
        }

        private void HandlePlayerDowned(OnPlayerDowned evt)
        {
            _downedPlayers.Add(evt.PlayerId);
            _allPlayers.Add(evt.PlayerId);

            if (_allPlayers.Count > 0 && _downedPlayers.Count >= _allPlayers.Count)
            {
                _allDowned = true;
                float timer = _config != null ? _config.RespawnTimerDuration : 15f;
                _respawnTimer = timer;
                _eventBus.Publish(new OnAllPlayersDowned(timer));
            }
        }

        private void HandlePlayerRevived(OnPlayerRevived evt)
        {
            _downedPlayers.Remove(evt.PlayerId);
            _activeRevives.Remove(evt.PlayerId);

            if (_downedPlayers.Count == 0)
            {
                _allDowned = false;
            }
        }

        private void HandlePlayerConnected(OnPlayerConnected evt)
        {
            _allPlayers.Add(evt.PlayerId);
        }

        private void HandlePlayerDisconnected(OnPlayerDisconnected evt)
        {
            _allPlayers.Remove(evt.PlayerId);
            _downedPlayers.Remove(evt.PlayerId);
            _activeRevives.Remove(evt.PlayerId);
        }

        private void HandlePhaseChanged(OnPhaseChanged evt)
        {
            if (evt.NewPhase == PhaseType.Prepare)
            {
                _downedPlayers.Clear();
                _activeRevives.Clear();
                _allDowned = false;
            }
        }

        private void OnDestroy()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe(_onPlayerDowned);
                _eventBus.Unsubscribe(_onPlayerRevived);
                _eventBus.Unsubscribe(_onPlayerConnected);
                _eventBus.Unsubscribe(_onPlayerDisconnected);
                _eventBus.Unsubscribe(_onPhaseChanged);
            }

            if (ServiceLocator.Instance != null)
                ServiceLocator.Instance.Unregister<IPlayerDownController>();
        }

        private class ReviveState
        {
            public readonly int ReviverId;
            public float Progress;

            public ReviveState(int reviverId, float progress)
            {
                ReviverId = reviverId;
                Progress = progress;
            }
        }
    }
}
