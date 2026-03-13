using System;
using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewBalanceProfile", menuName = "Ragdoll Realms/Content/Balance Profile")]
    public class BalanceProfileDefinition : ContentDefinition
    {
        [Serializable]
        public struct BalanceEntry
        {
            public string Key;
            public float Value;
        }

        [Serializable]
        public struct BalanceCurveEntry
        {
            public string Key;
            public AnimationCurve Curve;
        }

        [Header("Flat Values")]
        [SerializeField] private List<BalanceEntry> _values = new();

        [Header("Curves")]
        [SerializeField] private List<BalanceCurveEntry> _curves = new();

        private Dictionary<string, float> _valueLookup;
        private Dictionary<string, AnimationCurve> _curveLookup;

        public float GetValue(string key, float defaultValue = 0f)
        {
            EnsureLookups();
            return _valueLookup.TryGetValue(key, out var v) ? v : defaultValue;
        }

        public float EvaluateCurve(string key, float t, float defaultValue = 0f)
        {
            EnsureLookups();
            return _curveLookup.TryGetValue(key, out var curve) ? curve.Evaluate(t) : defaultValue;
        }

        private void EnsureLookups()
        {
            if (_valueLookup == null)
            {
                _valueLookup = new Dictionary<string, float>();
                foreach (var entry in _values)
                    _valueLookup[entry.Key] = entry.Value;
            }
            if (_curveLookup == null)
            {
                _curveLookup = new Dictionary<string, AnimationCurve>();
                foreach (var entry in _curves)
                    _curveLookup[entry.Key] = entry.Curve;
            }
        }

        private void OnValidate()
        {
            _valueLookup = null;
            _curveLookup = null;
        }
    }
}
