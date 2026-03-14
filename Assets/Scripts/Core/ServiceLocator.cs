using System;
using System.Collections.Generic;
using UnityEngine;

namespace RagdollRealms.Core
{
    [DefaultExecutionOrder(-100)]
    public sealed class ServiceLocator : MonoBehaviour, IServiceLocator
    {
        public static ServiceLocator Instance { get; private set; }

        private readonly Dictionary<Type, object> _services = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        public void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Overwriting existing service: {type.Name}");
            }
            _services[type] = service;
        }

        public void Unregister<T>() where T : class
        {
            _services.Remove(typeof(T));
        }

        public T Get<T>() where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return (T)service;
            }
            throw new InvalidOperationException(
                $"[ServiceLocator] Service not registered: {type.Name}");
        }

        public bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj))
            {
                service = (T)obj;
                return true;
            }
            service = null;
            return false;
        }

        public bool Has<T>() where T : class
        {
            return _services.ContainsKey(typeof(T));
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                _services.Clear();
                Instance = null;
            }
        }
    }
}
