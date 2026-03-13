using UnityEngine;

namespace RagdollRealms.Core
{
    public interface ISkillEffect
    {
        void Activate(GameObject owner);
        void Deactivate(GameObject owner);
        string GetDescription();
    }
}
