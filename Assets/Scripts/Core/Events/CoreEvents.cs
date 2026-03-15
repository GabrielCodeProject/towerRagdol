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
        public readonly PhaseType PreviousPhase;
        public readonly PhaseType NewPhase;

        public OnPhaseChanged(PhaseType previousPhase, PhaseType newPhase)
        {
            PreviousPhase = previousPhase;
            NewPhase = newPhase;
        }
    }

    public readonly struct OnPhaseTimerUpdate
    {
        public readonly PhaseType Phase;
        public readonly float RemainingTime;
        public readonly float TotalTime;

        public OnPhaseTimerUpdate(PhaseType phase, float remainingTime, float totalTime)
        {
            Phase = phase;
            RemainingTime = remainingTime;
            TotalTime = totalTime;
        }
    }

    public readonly struct OnGameSessionEnd
    {
        public readonly bool IsVictory;
        public readonly int WavesCompleted;
        public readonly int EnemiesKilled;
        public readonly int BossesDefeated;
        public readonly float TimePlayed;

        public OnGameSessionEnd(bool isVictory, int wavesCompleted, int enemiesKilled, int bossesDefeated, float timePlayed)
        {
            IsVictory = isVictory;
            WavesCompleted = wavesCompleted;
            EnemiesKilled = enemiesKilled;
            BossesDefeated = bossesDefeated;
            TimePlayed = timePlayed;
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

    public readonly struct OnPrepareWarning
    {
        public readonly float RemainingTime;
        public readonly int WarningIndex;

        public OnPrepareWarning(float remainingTime, int warningIndex)
        {
            RemainingTime = remainingTime;
            WarningIndex = warningIndex;
        }
    }

    public readonly struct OnPlayerReadyChanged
    {
        public readonly int PlayerId;
        public readonly bool IsReady;
        public readonly int ReadyCount;
        public readonly int TotalPlayers;

        public OnPlayerReadyChanged(int playerId, bool isReady, int readyCount, int totalPlayers)
        {
            PlayerId = playerId;
            IsReady = isReady;
            ReadyCount = readyCount;
            TotalPlayers = totalPlayers;
        }
    }

    public readonly struct OnWaveAnnouncement
    {
        public readonly int WaveNumber;
        public readonly int EnemyCount;

        public OnWaveAnnouncement(int waveNumber, int enemyCount)
        {
            WaveNumber = waveNumber;
            EnemyCount = enemyCount;
        }
    }

    public readonly struct OnTransitionReport
    {
        public readonly int WaveNumber;
        public readonly int EnemiesKilled;
        public readonly int StructuresDamaged;

        public OnTransitionReport(int waveNumber, int enemiesKilled, int structuresDamaged)
        {
            WaveNumber = waveNumber;
            EnemiesKilled = enemiesKilled;
            StructuresDamaged = structuresDamaged;
        }
    }

    public readonly struct OnDifficultyEscalation
    {
        public readonly int NextWaveNumber;

        public OnDifficultyEscalation(int nextWaveNumber)
        {
            NextWaveNumber = nextWaveNumber;
        }
    }

    public readonly struct OnPlayerReviveStarted
    {
        public readonly int ReviverId;
        public readonly int DownedPlayerId;

        public OnPlayerReviveStarted(int reviverId, int downedPlayerId)
        {
            ReviverId = reviverId;
            DownedPlayerId = downedPlayerId;
        }
    }

    public readonly struct OnPlayerReviveProgress
    {
        public readonly int DownedPlayerId;
        public readonly float Progress;

        public OnPlayerReviveProgress(int downedPlayerId, float progress)
        {
            DownedPlayerId = downedPlayerId;
            Progress = progress;
        }
    }

    public readonly struct OnPlayerRevived
    {
        public readonly int PlayerId;
        public readonly int ReviverId;

        public OnPlayerRevived(int playerId, int reviverId)
        {
            PlayerId = playerId;
            ReviverId = reviverId;
        }
    }

    public readonly struct OnPlayerReadyRequested
    {
        public readonly int PlayerId;

        public OnPlayerReadyRequested(int playerId)
        {
            PlayerId = playerId;
        }
    }

    public readonly struct OnAllPlayersDowned
    {
        public readonly float RespawnTimer;

        public OnAllPlayersDowned(float respawnTimer)
        {
            RespawnTimer = respawnTimer;
        }
    }

    public readonly struct OnVictoryCelebration
    {
        public readonly bool EndlessModeUnlocked;

        public OnVictoryCelebration(bool endlessModeUnlocked)
        {
            EndlessModeUnlocked = endlessModeUnlocked;
        }
    }
}
