using System;
using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewSkillTree", menuName = "Ragdoll Realms/Content/Skill Tree Definition")]
    public class SkillTreeDefinition : ContentDefinition
    {
        [Serializable]
        public struct SkillNode
        {
            public string NodeId;
            public SkillDefinition Skill;
            public Vector2 LayoutPosition;
            public List<string> PrerequisiteNodeIds;
            public int RequiredLevel;
        }

        [Header("Skill Tree")]
        [SerializeField] private List<SkillNode> _nodes = new();

        public IReadOnlyList<SkillNode> Nodes => _nodes;

        public SkillNode? GetNode(string nodeId)
        {
            foreach (var node in _nodes)
            {
                if (node.NodeId == nodeId) return node;
            }
            return null;
        }
    }
}
