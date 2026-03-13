using System;
using System.Collections.Generic;
using UnityEngine;

namespace RagdollRealms.Core
{
    public sealed class EventBus : MonoBehaviour, IEventBus
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();

        private void Awake()
        {
            ServiceLocator.Instance.Register<IEventBus>(this);
        }

        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var existing))
            {
                _handlers[type] = Delegate.Combine(existing, handler);
            }
            else
            {
                _handlers[type] = handler;
            }
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var existing))
            {
                var result = Delegate.Remove(existing, handler);
                if (result == null)
                    _handlers.Remove(type);
                else
                    _handlers[type] = result;
            }
        }

        public void Publish<T>(T eventData)
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var handler))
            {
                ((Action<T>)handler)?.Invoke(eventData);
            }
        }

        private void OnDestroy()
        {
            _handlers.Clear();
        }
    }
}
