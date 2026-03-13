using UnityEngine;

namespace RagdollRealms.Core.Events
{
    public readonly struct OnEnemyKilled
    {
        public readonly int EnemyId;
        public readonly Vector3 Position;
        public readonly string EnemyDefinitionId;
        public readonly int KillerPlayerId;

        public OnEnemyKilled(int enemyId, Vector3 position, string enemyDefinitionId, int killerPlayerId)
        {
            EnemyId = enemyId;
            Position = position;
            EnemyDefinitionId = enemyDefinitionId;
            KillerPlayerId = killerPlayerId;
        }
    }

    public readonly struct OnWaveStart
    {
        public readonly int WaveNumber;
        public readonly int EnemyCount;

        public OnWaveStart(int waveNumber, int enemyCount)
        {
            WaveNumber = waveNumber;
            EnemyCount = enemyCount;
        }
    }

    public readonly struct OnWaveEnd
    {
        public readonly int WaveNumber;
        public readonly bool AllEnemiesDefeated;

        public OnWaveEnd(int waveNumber, bool allEnemiesDefeated)
        {
            WaveNumber = waveNumber;
            AllEnemiesDefeated = allEnemiesDefeated;
        }
    }

    public readonly struct OnBossDefeated
    {
        public readonly string BossDefinitionId;
        public readonly int WaveNumber;

        public OnBossDefeated(string bossDefinitionId, int waveNumber)
        {
            BossDefinitionId = bossDefinitionId;
            WaveNumber = waveNumber;
        }
    }

    public readonly struct OnCoreHit
    {
        public readonly float Damage;
        public readonly float RemainingHp;
        public readonly int AttackerId;

        public OnCoreHit(float damage, float remainingHp, int attackerId)
        {
            Damage = damage;
            RemainingHp = remainingHp;
            AttackerId = attackerId;
        }
    }

    public readonly struct OnItemCrafted
    {
        public readonly string ItemDefinitionId;
        public readonly int PlayerId;
        public readonly int Quality;

        public OnItemCrafted(string itemDefinitionId, int playerId, int quality)
        {
            ItemDefinitionId = itemDefinitionId;
            PlayerId = playerId;
            Quality = quality;
        }
    }

    public readonly struct OnEnhancementAttempt
    {
        public readonly string ItemDefinitionId;
        public readonly int FromLevel;
        public readonly int ToLevel;
        public readonly bool Success;

        public OnEnhancementAttempt(string itemDefinitionId, int fromLevel, int toLevel, bool success)
        {
            ItemDefinitionId = itemDefinitionId;
            FromLevel = fromLevel;
            ToLevel = toLevel;
            Success = success;
        }
    }

    public readonly struct OnPlayerDowned
    {
        public readonly int PlayerId;
        public readonly Vector3 Position;

        public OnPlayerDowned(int playerId, Vector3 position)
        {
            PlayerId = playerId;
            Position = position;
        }
    }

    public readonly struct OnPhaseChanged
    {
        public readonly string PreviousPhase;
        public readonly string NewPhase;

        public OnPhaseChanged(string previousPhase, string newPhase)
        {
            PreviousPhase = previousPhase;
            NewPhase = newPhase;
        }
    }

    public readonly struct OnChunkLoaded
    {
        public readonly Vector2Int ChunkCoord;

        public OnChunkLoaded(Vector2Int chunkCoord)
        {
            ChunkCoord = chunkCoord;
        }
    }

    public readonly struct OnChunkUnloaded
    {
        public readonly Vector2Int ChunkCoord;

        public OnChunkUnloaded(Vector2Int chunkCoord)
        {
            ChunkCoord = chunkCoord;
        }
    }

    public readonly struct OnPlayerConnected
    {
        public readonly int PlayerId;

        public OnPlayerConnected(int playerId)
        {
            PlayerId = playerId;
        }
    }

    public readonly struct OnPlayerDisconnected
    {
        public readonly int PlayerId;

        public OnPlayerDisconnected(int playerId)
        {
            PlayerId = playerId;
        }
    }
}
