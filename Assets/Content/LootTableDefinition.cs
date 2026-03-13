using System;
using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewLootTable", menuName = "Ragdoll Realms/Content/Loot Table Definition")]
    public class LootTableDefinition : ContentDefinition
    {
        [Serializable]
        public struct LootEntry
        {
            public ItemDefinition Item;
            public float Weight;
            public int MinQuantity;
            public int MaxQuantity;
            public List<string> RequiredTags;
            public int MinTier;
            public int MaxTier;
            [Tooltip("Reference another loot table instead of an item")]
            public LootTableDefinition NestedTable;
        }

        [Header("Loot Table")]
        [SerializeField] private List<LootEntry> _entries = new();
        [SerializeField] private int _minDrops = 1;
        [SerializeField] private int _maxDrops = 3;

        public IReadOnlyList<LootEntry> Entries => _entries;
        public int MinDrops => _minDrops;
        public int MaxDrops => _maxDrops;
    }
}
