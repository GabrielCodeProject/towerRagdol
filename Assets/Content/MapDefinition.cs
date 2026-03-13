using System;
using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewMap", menuName = "Ragdoll Realms/Content/Map Definition")]
    public class MapDefinition : ContentDefinition
    {
        [Serializable]
        public struct SpawnPointSlot
        {
            public string SlotId;
            public Vector2 Position;
            public float Radius;
        }

        [Serializable]
        public struct PoiSlot
        {
            public string SlotId;
            public Vector2 Position;
            public List<string> AllowedPoiTags;
        }

        [Serializable]
        public struct BiomeZone
        {
            public string BiomeId;
            public float InnerRadius;
            public float OuterRadius;
            public List<string> AvailableResourceTags;
        }

        [Header("Map Layout")]
        [SerializeField] private Vector2 _corePosition;
        [SerializeField] private List<SpawnPointSlot> _spawnPoints = new();
        [SerializeField] private List<BiomeZone> _biomeZones = new();
        [SerializeField] private List<PoiSlot> _poiSlots = new();

        [Header("Boss")]
        [SerializeField] private List<Vector2> _bossAltarLocations = new();

        [Header("Rules")]
        [SerializeField] private List<string> _allowedEnemyTags = new();
        [SerializeField] private List<string> _allowedTowerTags = new();

        public Vector2 CorePosition => _corePosition;
        public IReadOnlyList<SpawnPointSlot> SpawnPoints => _spawnPoints;
        public IReadOnlyList<BiomeZone> BiomeZones => _biomeZones;
        public IReadOnlyList<PoiSlot> PoiSlots => _poiSlots;
        public IReadOnlyList<Vector2> BossAltarLocations => _bossAltarLocations;
        public IReadOnlyList<string> AllowedEnemyTags => _allowedEnemyTags;
        public IReadOnlyList<string> AllowedTowerTags => _allowedTowerTags;
    }
}
