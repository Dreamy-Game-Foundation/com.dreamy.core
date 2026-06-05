using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// MonoBehaviour FSM. Auto-discovers all <see cref="BaseState"/> children on Awake.
    /// O(1) state lookup via Dictionary&lt;Type, IState&gt;.
    /// </summary>
    public class StateMachine : MonoBehaviour
    {
        readonly Dictionary<Type, IState> _states = new();
        readonly IState _null = new NullState();

        IState _currentState;
        IState _previousState;
        IState _initialState;

        public string CurrentStateName  => _currentState?.GetType().Name  ?? "None";
        public string PreviousStateName => _previousState?.GetType().Name ?? "None";

        protected virtual void Awake()
        {
            var children = GetComponentsInChildren<BaseState>(includeInactive: true);
            foreach (var state in children)
            {
                state.SetMachine(this);
                _states[state.GetType()] = state;
                state.OnInitialize();
            }
        }

        /// <summary>Sets the starting state and calls OnEnter.</summary>
        public void Initialize<TState>() where TState : BaseState
        {
            _currentState = GetStateOrNull(typeof(TState));
            _initialState = _currentState;
            _currentState.OnEnter();
        }

        /// <summary>Transitions to TState. No-op if already in TState.</summary>
        public void ChangeState<TState>(IStateData data = null) where TState : BaseState
        {
            var next = GetStateOrNull(typeof(TState));
            if (_currentState == next) return;

            _currentState?.OnExit();
            _previousState = _currentState;
            _currentState  = next;
            _currentState.OnEnter(data);
        }

        /// <summary>Returns to the state active before the current one.</summary>
        public void BackToPrevious()
        {
            if (_previousState == null) return;
            _currentState?.OnExit();
            (_currentState, _previousState) = (_previousState, _currentState);
            _currentState.OnEnter();
        }

        /// <summary>Returns to the state set in <see cref="Initialize{TState}"/>.</summary>
        public void BackToInitial()
        {
            if (_initialState == null || _currentState == _initialState) return;
            _currentState?.OnExit();
            _previousState = _currentState;
            _currentState  = _initialState;
            _currentState.OnEnter();
        }

        protected virtual void Update() => _currentState?.OnExecute();

        public bool IsCurrentState<TState>() where TState : BaseState => _currentState is TState;
        public bool HasState<TState>()        where TState : BaseState => _states.ContainsKey(typeof(TState));

        public TState GetState<TState>() where TState : BaseState
        {
            _states.TryGetValue(typeof(TState), out var state);
            return state as TState;
        }

        IState GetStateOrNull(Type type) =>
            _states.TryGetValue(type, out var s) ? s : _null;
    }
}
