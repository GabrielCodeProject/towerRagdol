using System;
using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewBuildingPiece", menuName = "Ragdoll Realms/Content/Building Piece Definition")]
    public class BuildingPieceDefinition : ContentDefinition
    {
        [Serializable]
        public struct BuildCost
        {
            public ItemDefinition Resource;
            public int Amount;
        }

        [Header("Building Properties")]
        [SerializeField] private float _maxHp = 100f;
        [SerializeField] private float _structuralIntegrity = 1f;
        [SerializeField] private Vector2Int _gridSize = Vector2Int.one;
        [SerializeField] private bool _supportsTowerPlacement;

        [Header("Cost")]
        [SerializeField] private List<BuildCost> _buildCosts = new();

        [Header("Visuals")]
        [SerializeField] private GameObject _prefab;
        [SerializeField] private GameObject _ghostPrefab;

        public float MaxHp => _maxHp;
        public float StructuralIntegrity => _structuralIntegrity;
        public Vector2Int GridSize => _gridSize;
        public bool SupportsTowerPlacement => _supportsTowerPlacement;
        public IReadOnlyList<BuildCost> BuildCosts => _buildCosts;
        public GameObject Prefab => _prefab;
        public GameObject GhostPrefab => _ghostPrefab;
    }
}
