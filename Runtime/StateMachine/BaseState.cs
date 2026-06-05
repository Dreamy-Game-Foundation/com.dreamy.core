using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// Abstract MonoBehaviour base for all states managed by <see cref="StateMachine"/>.
    /// Place as a child GameObject of the StateMachine GameObject.
    /// </summary>
    public abstract class BaseState : MonoBehaviour, IState
    {
        protected StateMachine Machine { get; private set; }

        internal void SetMachine(StateMachine machine) => Machine = machine;

        public virtual void OnInitialize() { }
        public virtual void OnEnter(IStateData data = null) { }
        public virtual void OnExecute() { }
        public virtual void OnExit() { }
    }
}
