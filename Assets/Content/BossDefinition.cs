using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewBoss", menuName = "Ragdoll Realms/Content/Boss Definition")]
    public class BossDefinition : ContentDefinition
    {
        [Header("Boss Stats")]
        [SerializeField] private float _maxHp = 5000f;
        [SerializeField] private float _baseDamage = 50f;
        [SerializeField] private int _phaseCount = 3;
        [SerializeField] private int _requiredFragments = 5;

        [Header("Scaling")]
        [SerializeField] private float _coopHpMultiplierPerPlayer = 0.5f;
        [SerializeField] private float _coopDamageMultiplierPerPlayer = 0.25f;

        [Header("Visuals")]
        [SerializeField] private GameObject _prefab;

        [Header("Phases")]
        [SerializeField] private List<ScriptableObject> _phaseSOs = new();

        public float MaxHp => _maxHp;
        public float BaseDamage => _baseDamage;
        public int PhaseCount => _phaseCount;
        public int RequiredFragments => _requiredFragments;
        public float CoopHpMultiplierPerPlayer => _coopHpMultiplierPerPlayer;
        public float CoopDamageMultiplierPerPlayer => _coopDamageMultiplierPerPlayer;
        public GameObject Prefab => _prefab;

        public IBossPhase GetPhase(int index)
        {
            if (index < 0 || index >= _phaseSOs.Count) return null;
            return _phaseSOs[index] as IBossPhase;
        }
    }
}
