using System.IO;
using System.Linq;
using RagdollRealms.Core;
using RagdollRealms.Core.Data;
using UnityEngine;

namespace RagdollRealms.Systems
{
    public class SaveManager : MonoBehaviour, ISaveManager
    {
        private string SaveDirectory => Path.Combine(Application.persistentDataPath, "Saves");

        private SessionSaveData _currentSession;

        public bool HasActiveSession => _currentSession != null;
        public int MapSeed => _currentSession?.MapSeed ?? 0;
        public int WaveNumber => _currentSession?.WaveNumber ?? 0;
        public float CoreHp => _currentSession?.CoreHp ?? 0f;
        public string MapDefinitionId => _currentSession?.MapDefinitionId ?? string.Empty;

        private void Awake()
        {
            ServiceLocator.Instance.Register<ISaveManager>(this);
            Directory.CreateDirectory(SaveDirectory);
        }

        public void Save(string slotName)
        {
            if (_currentSession == null)
            {
                Debug.LogWarning("[SaveManager] No active session to save.");
                return;
            }

            _currentSession.SaveTimestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var json = JsonUtility.ToJson(_currentSession, true);
            var path = GetSavePath(slotName);
            File.WriteAllText(path, json);
            Debug.Log($"[SaveManager] Saved to: {path}");
        }

        public bool Load(string slotName)
        {
            var path = GetSavePath(slotName);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[SaveManager] Save not found: {slotName}");
                return false;
            }

            var json = File.ReadAllText(path);
            _currentSession = JsonUtility.FromJson<SessionSaveData>(json);
            Debug.Log($"[SaveManager] Loaded: {slotName} (Wave {_currentSession.WaveNumber})");
            return true;
        }

        public bool HasSave(string slotName) => File.Exists(GetSavePath(slotName));

        public void DeleteSave(string slotName)
        {
            var path = GetSavePath(slotName);
            if (File.Exists(path)) File.Delete(path);
        }

        public string[] GetSaveSlots()
        {
            if (!Directory.Exists(SaveDirectory)) return System.Array.Empty<string>();
            return Directory.GetFiles(SaveDirectory, "*.json")
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();
        }

        public void NewSession(int seed, string mapDefinitionId, float coreHp)
        {
            _currentSession = new SessionSaveData
            {
                MapSeed = seed,
                WaveNumber = 0,
                CoreHp = coreHp,
                MapDefinitionId = mapDefinitionId
            };
        }

        public void SetWaveNumber(int wave)
        {
            if (_currentSession == null) return;
            _currentSession.WaveNumber = wave;
        }

        public void SetCoreHp(float hp)
        {
            if (_currentSession == null) return;
            _currentSession.CoreHp = hp;
        }

        public void AddBuilding(BuildingSaveData building)
        {
            if (_currentSession == null) return;
            _currentSession.Buildings.Add(building);
        }

        public void AddTower(TowerSaveData tower)
        {
            if (_currentSession == null) return;
            _currentSession.Towers.Add(tower);
        }

        private string GetSavePath(string slotName) => Path.Combine(SaveDirectory, $"{slotName}.json");

        private void OnDestroy()
        {
            if (ServiceLocator.Instance != null)
                ServiceLocator.Instance.Unregister<ISaveManager>();
        }
    }
}
