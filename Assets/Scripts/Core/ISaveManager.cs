using RagdollRealms.Core.Data;

namespace RagdollRealms.Core
{
    public interface ISaveManager
    {
        void Save(string slotName);
        bool Load(string slotName);
        bool HasSave(string slotName);
        void DeleteSave(string slotName);
        string[] GetSaveSlots();
        void NewSession(int seed, string mapDefinitionId, float coreHp);

        // Read-only accessors — do not expose mutable SessionSaveData directly
        int MapSeed { get; }
        int WaveNumber { get; }
        float CoreHp { get; }
        string MapDefinitionId { get; }
        bool HasActiveSession { get; }

        void SetWaveNumber(int wave);
        void SetCoreHp(float hp);
        void AddBuilding(BuildingSaveData building);
        void AddTower(TowerSaveData tower);
    }
}
