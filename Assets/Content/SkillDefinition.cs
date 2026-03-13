using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Ragdoll Realms/Content/Skill Definition")]
    public class SkillDefinition : ContentDefinition
    {
        [Header("Skill")]
        [SerializeField] private int _maxLevel = 5;
        [SerializeField] private int _skillPointCost = 1;
        [SerializeField] private bool _isPassive;

        [Header("Behavior")]
        [SerializeField, SerializeReference] private ScriptableObject _effectSO;

        public int MaxLevel => _maxLevel;
        public int SkillPointCost => _skillPointCost;
        public bool IsPassive => _isPassive;
        public ISkillEffect Effect => _effectSO as ISkillEffect;
    }
}
