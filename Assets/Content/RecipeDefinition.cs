using System;
using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Ragdoll Realms/Content/Recipe Definition")]
    public class RecipeDefinition : ContentDefinition
    {
        [Serializable]
        public struct Ingredient
        {
            public ItemDefinition Item;
            public int Quantity;
        }

        [Header("Recipe")]
        [SerializeField] private List<Ingredient> _ingredients = new();
        [SerializeField] private ItemDefinition _result;
        [SerializeField] private int _resultQuantity = 1;
        [SerializeField] private int _requiredStationTier = 1;
        [SerializeField] private float _craftTime = 2f;
        [SerializeField] private bool _discoverable = true;

        public IReadOnlyList<Ingredient> Ingredients => _ingredients;
        public ItemDefinition Result => _result;
        public int ResultQuantity => _resultQuantity;
        public int RequiredStationTier => _requiredStationTier;
        public float CraftTime => _craftTime;
        public bool Discoverable => _discoverable;
    }
}
