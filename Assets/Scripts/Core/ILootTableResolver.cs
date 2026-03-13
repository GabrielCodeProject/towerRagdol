using System.Collections.Generic;
using RagdollRealms.Content;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Core
{
    public interface ILootTableResolver
    {
        List<LootDrop> Roll(LootTableDefinition table, SeededRandom random, IReadOnlyList<string> contextTags = null, int maxDepth = 5);
    }
}
