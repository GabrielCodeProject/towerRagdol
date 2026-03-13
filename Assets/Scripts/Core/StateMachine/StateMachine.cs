using System;
using System.Collections.Generic;
using UnityEngine;

namespace RagdollRealms.Core.StateMachine
{
    public class StateMachine
    {
        private IState _currentState;
        private readonly Dictionary<Type, IState> _states = new();

        public IState CurrentState => _currentState;

        public void AddState(IState state)
        {
            _states[state.GetType()] = state;
        }

        public void SetState<T>() where T : IState
        {
            var type = typeof(T);
            if (!_states.TryGetValue(type, out var newState))
            {
                Debug.LogError($"[StateMachine] State not registered: {type.Name}");
                return;
            }

            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public void SetState(IState state)
        {
            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public bool IsInState<T>() where T : IState
        {
            return _currentState is T;
        }
    }
}
