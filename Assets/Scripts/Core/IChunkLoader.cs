using UnityEngine;

namespace RagdollRealms.Core
{
    public interface IChunkLoader
    {
        void LoadChunk(Vector2Int chunkCoord);
        void UnloadChunk(Vector2Int chunkCoord);
        bool IsChunkLoaded(Vector2Int chunkCoord);
        Vector2Int WorldToChunk(Vector3 worldPosition);
    }
}
