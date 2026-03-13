using System;
using System.Collections.Generic;
using RagdollRealms.Core.Data;
using UnityEngine;

namespace RagdollRealms.Core
{
    public class ContentRegistry<T> : IContentRegistry<T> where T : ContentDefinition
    {
        private readonly Dictionary<string, T> _byId = new();
        private readonly List<T> _all = new();

        public int Count => _all.Count;

        public void LoadFromResources(string path)
        {
            var assets = Resources.LoadAll<T>(path);
            foreach (var asset in assets)
            {
                Register(asset);
            }
            Debug.Log($"[ContentRegistry<{typeof(T).Name}>] Loaded {assets.Length} from Resources/{path}");
        }

        public void Register(T definition)
        {
            if (definition == null) return;

            if (string.IsNullOrEmpty(definition.Id))
            {
                Debug.LogWarning($"[ContentRegistry<{typeof(T).Name}>] Skipping asset with empty ID: {definition.name}");
                return;
            }

            if (_byId.ContainsKey(definition.Id))
            {
                Debug.LogWarning($"[ContentRegistry<{typeof(T).Name}>] Duplicate ID '{definition.Id}', overwriting.");
            }

            _byId[definition.Id] = definition;
            if (!_all.Contains(definition))
                _all.Add(definition);
        }

        public void Unregister(string id)
        {
            if (_byId.TryGetValue(id, out var def))
            {
                _byId.Remove(id);
                _all.Remove(def);
            }
        }

        public T GetById(string id)
        {
            if (_byId.TryGetValue(id, out var def))
                return def;
            throw new KeyNotFoundException(
                $"[ContentRegistry<{typeof(T).Name}>] No definition found with ID: {id}");
        }

        public bool TryGetById(string id, out T definition)
        {
            return _byId.TryGetValue(id, out definition);
        }

        public IReadOnlyList<T> GetAll() => _all;

        public IReadOnlyList<T> GetByTier(int tier)
        {
            var results = new List<T>();
            foreach (var def in _all)
            {
                if (def.Tier == tier) results.Add(def);
            }
            return results;
        }

        public IReadOnlyList<T> GetByTag(string tag)
        {
            var results = new List<T>();
            foreach (var def in _all)
            {
                if (def.HasTag(tag)) results.Add(def);
            }
            return results;
        }

        public IReadOnlyList<T> GetByTags(IEnumerable<string> tags, bool matchAll = false)
        {
            var results = new List<T>();
            foreach (var def in _all)
            {
                if (matchAll ? def.HasAllTags(tags) : def.HasAnyTag(tags))
                    results.Add(def);
            }
            return results;
        }

        public void Clear()
        {
            _byId.Clear();
            _all.Clear();
        }
    }
}
