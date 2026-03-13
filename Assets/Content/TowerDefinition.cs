using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewTower", menuName = "Ragdoll Realms/Content/Tower Definition")]
    public class TowerDefinition : ContentDefinition
    {
        [Header("Tower Stats")]
        [SerializeField] private float _maxHp = 200f;
        [SerializeField] private float _attackDamage = 15f;
        [SerializeField] private float _attackRange = 8f;
        [SerializeField] private float _attackRate = 1f;
        [SerializeField] private int _maxUpgradeLevel = 3;

        [Header("Visuals")]
        [SerializeField] private GameObject _prefab;
        [SerializeField] private GameObject _projectilePrefab;

        [Header("Targeting")]
        [SerializeField, SerializeReference] private ScriptableObject _targetingStrategySO;

        public float MaxHp => _maxHp;
        public float AttackDamage => _attackDamage;
        public float AttackRange => _attackRange;
        public float AttackRate => _attackRate;
        public int MaxUpgradeLevel => _maxUpgradeLevel;
        public GameObject Prefab => _prefab;
        public GameObject ProjectilePrefab => _projectilePrefab;
        public ITargetingStrategy TargetingStrategy => _targetingStrategySO as ITargetingStrategy;
    }
}
