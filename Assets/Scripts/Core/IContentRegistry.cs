using System.Collections.Generic;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Core
{
    public interface IContentRegistry<T> where T : ContentDefinition
    {
        T GetById(string id);
        IReadOnlyList<T> GetAll();
        IReadOnlyList<T> GetByTier(int tier);
        IReadOnlyList<T> GetByTag(string tag);
        IReadOnlyList<T> GetByTags(IEnumerable<string> tags, bool matchAll = false);
        bool TryGetById(string id, out T definition);
        int Count { get; }
    }
}
