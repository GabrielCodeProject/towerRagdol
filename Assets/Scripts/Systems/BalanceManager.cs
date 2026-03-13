using RagdollRealms.Content;
using RagdollRealms.Core;
using UnityEngine;

namespace RagdollRealms.Systems
{
    public class BalanceManager : MonoBehaviour, IBalanceManager
    {
        [SerializeField] private BalanceProfileDefinition _defaultProfile;

        private IContentRegistry<BalanceProfileDefinition> _registry;
        private BalanceProfileDefinition _activeProfile;

        public string ActiveProfileId => _activeProfile != null ? _activeProfile.Id : string.Empty;

        private void Awake()
        {
            ServiceLocator.Instance.Register<IBalanceManager>(this);
        }

        private void Start()
        {
            if (ServiceLocator.Instance.TryGet<IContentRegistry<BalanceProfileDefinition>>(out var registry))
            {
                _registry = registry;
            }

            _activeProfile = _defaultProfile;
        }

        public float GetValue(string key, float defaultValue = 0f)
        {
            if (_activeProfile == null) return defaultValue;
            return _activeProfile.GetValue(key, defaultValue);
        }

        public float EvaluateCurve(string key, float t, float defaultValue = 0f)
        {
            if (_activeProfile == null) return defaultValue;
            return _activeProfile.EvaluateCurve(key, t, defaultValue);
        }

        public void SetActiveProfile(string profileId)
        {
            if (_registry != null && _registry.TryGetById(profileId, out var profile))
            {
                _activeProfile = profile;
                Debug.Log($"[BalanceManager] Active profile set to: {profileId}");
            }
            else
            {
                Debug.LogWarning($"[BalanceManager] Profile not found: {profileId}");
            }
        }

        private void OnDestroy()
        {
            if (ServiceLocator.Instance != null)
                ServiceLocator.Instance.Unregister<IBalanceManager>();
        }
    }
}
