using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Core
{
    public interface ILootTableResolver
    {
        List<LootDrop> Roll(ScriptableObject table, SeededRandom random, IReadOnlyList<string> contextTags = null, int maxDepth = 5);
    }
}
