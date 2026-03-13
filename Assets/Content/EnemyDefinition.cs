using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewEnemy", menuName = "Ragdoll Realms/Content/Enemy Definition")]
    public class EnemyDefinition : ContentDefinition
    {
        [Header("Enemy Stats")]
        [SerializeField] private float _maxHp = 100f;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _attackDamage = 10f;
        [SerializeField] private float _attackRange = 1.5f;
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private int _pointCost = 1;

        [Header("Visuals")]
        [SerializeField] private GameObject _prefab;
        [SerializeField] private RuntimeAnimatorController _animator;

        [Header("Behavior")]
        [SerializeField, SerializeReference] private ScriptableObject _behaviorSO;

        public float MaxHp => _maxHp;
        public float MoveSpeed => _moveSpeed;
        public float AttackDamage => _attackDamage;
        public float AttackRange => _attackRange;
        public float AttackCooldown => _attackCooldown;
        public int PointCost => _pointCost;
        public GameObject Prefab => _prefab;
        public RuntimeAnimatorController Animator => _animator;
        public IEnemyBehavior Behavior => _behaviorSO as IEnemyBehavior;
    }
}
