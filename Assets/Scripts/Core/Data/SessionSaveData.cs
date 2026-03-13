using System;
using System.Collections.Generic;

namespace RagdollRealms.Core.Data
{
    [Serializable]
    public class SessionSaveData
    {
        public int MapSeed;
        public int WaveNumber;
        public float CoreHp;
        public string MapDefinitionId;
        public List<BuildingSaveData> Buildings = new();
        public List<TowerSaveData> Towers = new();
        public List<PlayerSaveData> Players = new();
        public List<string> ExploredPoiIds = new();
        public List<string> DefeatedBossIds = new();
        public long SaveTimestamp;
    }

    [Serializable]
    public class BuildingSaveData
    {
        public string DefinitionId;
        public float PositionX;
        public float PositionY;
        public float CurrentHp;
    }

    [Serializable]
    public class TowerSaveData
    {
        public string DefinitionId;
        public float PositionX;
        public float PositionY;
        public int UpgradeLevel;
        public float CurrentHp;
    }

    [Serializable]
    public class PlayerSaveData
    {
        public int PlayerId;
        public string ClassDefinitionId;
        public int Level;
        public float Xp;
        public List<InventorySlotData> Inventory = new();
        public List<string> UnlockedSkillIds = new();
    }

    [Serializable]
    public class InventorySlotData
    {
        public string ItemDefinitionId;
        public int Quantity;
        public int EnhanceLevel;
    }
}
