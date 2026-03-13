using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewSpell", menuName = "Ragdoll Realms/Content/Spell Definition")]
    public class SpellDefinition : ContentDefinition
    {
        [Header("Spell Stats")]
        [SerializeField] private float _baseDamage = 20f;
        [SerializeField] private float _manaCost = 10f;
        [SerializeField] private float _cooldown = 2f;
        [SerializeField] private float _range = 10f;
        [SerializeField] private float _areaOfEffect;
        [SerializeField] private int _maxLevel = 5;

        [Header("Visuals")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private GameObject _impactVfxPrefab;

        [Header("Behavior")]
        [SerializeField, SerializeReference] private ScriptableObject _effectSO;

        public float BaseDamage => _baseDamage;
        public float ManaCost => _manaCost;
        public float Cooldown => _cooldown;
        public float Range => _range;
        public float AreaOfEffect => _areaOfEffect;
        public int MaxLevel => _maxLevel;
        public GameObject ProjectilePrefab => _projectilePrefab;
        public GameObject ImpactVfxPrefab => _impactVfxPrefab;
        public ISpellEffect Effect => _effectSO as ISpellEffect;
    }
}
