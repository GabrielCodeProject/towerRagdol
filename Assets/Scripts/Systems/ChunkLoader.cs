using System.Collections.Generic;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using UnityEngine;

namespace RagdollRealms.Systems
{
    public class ChunkLoader : MonoBehaviour, IChunkLoader
    {
        [Header("Chunk Settings")]
        [SerializeField] private float _chunkSize = 32f;
        [SerializeField] private int _loadRadius = 2;

        private readonly HashSet<Vector2Int> _loadedChunks = new();
        private Vector2Int _lastPlayerChunk = new(int.MinValue, int.MinValue);

        private IEventBus _eventBus;

        private void Awake()
        {
            ServiceLocator.Instance.Register<IChunkLoader>(this);
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();
        }

        public void UpdatePlayerPosition(Vector3 worldPosition)
        {
            var current = WorldToChunk(worldPosition);
            if (current == _lastPlayerChunk) return;
            _lastPlayerChunk = current;

            var desired = new HashSet<Vector2Int>();
            for (int x = -_loadRadius; x <= _loadRadius; x++)
            {
                for (int y = -_loadRadius; y <= _loadRadius; y++)
                {
                    desired.Add(new Vector2Int(current.x + x, current.y + y));
                }
            }

            // Unload chunks no longer needed
            var toUnload = new List<Vector2Int>();
            foreach (var chunk in _loadedChunks)
            {
                if (!desired.Contains(chunk))
                    toUnload.Add(chunk);
            }
            foreach (var chunk in toUnload)
                UnloadChunk(chunk);

            // Load new chunks
            foreach (var chunk in desired)
            {
                if (!_loadedChunks.Contains(chunk))
                    LoadChunk(chunk);
            }
        }

        public void LoadChunk(Vector2Int chunkCoord)
        {
            if (_loadedChunks.Contains(chunkCoord)) return;
            _loadedChunks.Add(chunkCoord);
            _eventBus.Publish(new OnChunkLoaded(chunkCoord));
        }

        public void UnloadChunk(Vector2Int chunkCoord)
        {
            if (!_loadedChunks.Remove(chunkCoord)) return;
            _eventBus.Publish(new OnChunkUnloaded(chunkCoord));
        }

        public bool IsChunkLoaded(Vector2Int chunkCoord) => _loadedChunks.Contains(chunkCoord);

        public Vector2Int WorldToChunk(Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPosition.x / _chunkSize),
                Mathf.FloorToInt(worldPosition.y / _chunkSize));
        }

        private void OnDestroy()
        {
            if (ServiceLocator.Instance != null)
                ServiceLocator.Instance.Unregister<IChunkLoader>();
        }
    }
}
