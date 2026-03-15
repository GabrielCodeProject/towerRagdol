using System;
using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewCore", menuName = "RagdollRealms/Core")]
    public class CoreDefinition : ContentDefinition
    {
        [Header("Base Stats")]
        [SerializeField] private float _baseHp = 1000f;

        [Header("Healing Aura")]
        [SerializeField] private float _healingAuraRadius = 10f;
        [SerializeField] private float _healingAuraRate = 2f;

        [Header("Alarm")]
        [SerializeField] private float _alarmBaseRange = 20f;

        [Header("Prefabs")]
        [SerializeField] private GameObject _corePrefab;

        [Header("VFX")]
        [SerializeField] private GameObject[] _crackStagePrefabs;
        [SerializeField] private GameObject _shieldBubblePrefab;
        [SerializeField] private GameObject _auraParticlesPrefab;
        [SerializeField] private GameObject _alarmPulsePrefab;

        [Header("Upgrades")]
        [SerializeField] private List<CoreUpgradeTier> _upgradeTiers;

        public float BaseHp => _baseHp;
        public float HealingAuraRadius => _healingAuraRadius;
        public float HealingAuraRate => _healingAuraRate;
        public float AlarmBaseRange => _alarmBaseRange;
        public GameObject CorePrefab => _corePrefab;
        public GameObject[] CrackStagePrefabs => _crackStagePrefabs;
        public GameObject ShieldBubblePrefab => _shieldBubblePrefab;
        public GameObject AuraParticlesPrefab => _auraParticlesPrefab;
        public GameObject AlarmPulsePrefab => _alarmPulsePrefab;
        public IReadOnlyList<CoreUpgradeTier> UpgradeTiers => _upgradeTiers;

        [Serializable]
        public struct CoreUpgradeTier
        {
            public float HpMultiplier;
            public float ShieldCapacity;
            public float AuraRadiusMultiplier;
            public float AuraRateMultiplier;
            public float AlarmRangeMultiplier;
        }
    }
}
