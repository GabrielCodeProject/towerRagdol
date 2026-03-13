using UnityEngine;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Ragdoll Realms/Content/Item Definition")]
    public class ItemDefinition : ContentDefinition
    {
        [Header("Item Properties")]
        [SerializeField] private int _maxStackSize = 99;
        [SerializeField] private bool _isEquippable;
        [SerializeField] private bool _isConsumable;
        [SerializeField] private bool _isEnhanceable;
        [SerializeField] private int _maxEnhanceLevel = 25;
        [SerializeField] private float _baseValue;

        [Header("Visuals")]
        [SerializeField] private GameObject _worldPrefab;
        [SerializeField] private GameObject _equippedPrefab;

        public int MaxStackSize => _maxStackSize;
        public bool IsEquippable => _isEquippable;
        public bool IsConsumable => _isConsumable;
        public bool IsEnhanceable => _isEnhanceable;
        public int MaxEnhanceLevel => _maxEnhanceLevel;
        public float BaseValue => _baseValue;
        public GameObject WorldPrefab => _worldPrefab;
        public GameObject EquippedPrefab => _equippedPrefab;
    }
}
