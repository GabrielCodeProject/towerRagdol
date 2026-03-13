using System;
using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewClass", menuName = "Ragdoll Realms/Content/Class Definition")]
    public class ClassDefinition : ContentDefinition
    {
        [Serializable]
        public struct StatValue
        {
            public string StatId;
            public float BaseValue;
            public float GrowthPerLevel;
        }

        [Header("Base Stats")]
        [SerializeField] private List<StatValue> _baseStats = new();

        [Header("Passives")]
        [SerializeField] private List<ScriptableObject> _passiveAbilitySOs = new();

        [Header("Starting Gear")]
        [SerializeField] private List<ItemDefinition> _startingEquipment = new();

        [Header("Skill Tree")]
        [SerializeField] private SkillTreeDefinition _skillTree;

        public IReadOnlyList<StatValue> BaseStats => _baseStats;
        public IReadOnlyList<ItemDefinition> StartingEquipment => _startingEquipment;
        public SkillTreeDefinition SkillTree => _skillTree;

        public float GetBaseStat(string statId)
        {
            foreach (var stat in _baseStats)
            {
                if (stat.StatId == statId) return stat.BaseValue;
            }
            return 0f;
        }

        public float GetStatAtLevel(string statId, int level)
        {
            foreach (var stat in _baseStats)
            {
                if (stat.StatId == statId)
                    return stat.BaseValue + stat.GrowthPerLevel * (level - 1);
            }
            return 0f;
        }
    }
}
