using System.Collections.Generic;
using RagdollRealms.Content;
using RagdollRealms.Core;
using RagdollRealms.Core.Data;
using UnityEngine;

namespace RagdollRealms.Systems
{
    public class LootTableResolver : MonoBehaviour, ILootTableResolver
    {
        private void Awake()
        {
            ServiceLocator.Instance.Register<ILootTableResolver>(this);
        }

        public List<LootDrop> Roll(LootTableDefinition table, SeededRandom random, IReadOnlyList<string> contextTags = null, int maxDepth = 5)
        {
            var drops = new List<LootDrop>();
            if (table == null || random == null || maxDepth <= 0) return drops;

            int dropCount = random.Range(table.MinDrops, table.MaxDrops + 1);

            for (int i = 0; i < dropCount; i++)
            {
                var entry = SelectWeightedEntry(table.Entries, contextTags, random);
                if (entry == null) continue;

                var e = entry.Value;

                if (e.NestedTable != null)
                {
                    drops.AddRange(Roll(e.NestedTable, random, contextTags, maxDepth - 1));
                }
                else if (e.Item != null)
                {
                    drops.Add(new LootDrop
                    {
                        Item = e.Item,
                        Quantity = random.Range(e.MinQuantity, e.MaxQuantity + 1)
                    });
                }
            }

            return drops;
        }

        private static LootTableDefinition.LootEntry? SelectWeightedEntry(
            IReadOnlyList<LootTableDefinition.LootEntry> entries,
            IReadOnlyList<string> contextTags,
            SeededRandom random)
        {
            float totalWeight = 0f;
            var validEntries = new List<LootTableDefinition.LootEntry>();

            foreach (var entry in entries)
            {
                if (!PassesTagFilter(entry, contextTags)) continue;
                validEntries.Add(entry);
                totalWeight += entry.Weight;
            }

            if (validEntries.Count == 0 || totalWeight <= 0f) return null;

            float roll = random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var entry in validEntries)
            {
                cumulative += entry.Weight;
                if (roll <= cumulative) return entry;
            }

            return validEntries[^1];
        }

        private static bool PassesTagFilter(LootTableDefinition.LootEntry entry, IReadOnlyList<string> contextTags)
        {
            if (entry.RequiredTags == null || entry.RequiredTags.Count == 0) return true;
            if (contextTags == null || contextTags.Count == 0) return false;

            foreach (var requiredTag in entry.RequiredTags)
            {
                bool found = false;
                foreach (var tag in contextTags)
                {
                    if (tag == requiredTag) { found = true; break; }
                }
                if (!found) return false;
            }
            return true;
        }

        private void OnDestroy()
        {
            if (ServiceLocator.Instance != null)
                ServiceLocator.Instance.Unregister<ILootTableResolver>();
        }
    }
}
