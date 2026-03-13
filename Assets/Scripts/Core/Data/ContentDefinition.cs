using System.Collections.Generic;
using UnityEngine;

namespace RagdollRealms.Core.Data
{
    public abstract class ContentDefinition : ScriptableObject
    {
        [Header("Content Identity")]
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField, Range(1, 10)] private int _tier = 1;
        [SerializeField] private List<string> _tags = new();

        public string Id => _id;
        public string DisplayName => _displayName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public int Tier => _tier;
        public IReadOnlyList<string> Tags => _tags;

        public bool HasTag(string tag) => _tags.Contains(tag);

        public bool HasAllTags(IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                if (!_tags.Contains(tag)) return false;
            }
            return true;
        }

        public bool HasAnyTag(IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                if (_tags.Contains(tag)) return true;
            }
            return false;
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_id))
            {
                _id = name;
            }
        }
    }
}
