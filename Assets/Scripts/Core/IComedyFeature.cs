using System.Collections.Generic;

namespace RagdollRealms.Core
{
    public interface IComedyFeature
    {
        void OnActivate();
        void OnDeactivate();
        IReadOnlyList<string> GetTags();
    }
}
