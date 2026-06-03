using System;

namespace Dreamy.Core
{
    /// <summary>
    /// Binding token returned by MyEventBus&lt;T&gt;.Register().
    /// Hold a reference to this — pass to Unregister() to stop listening.
    /// </summary>
    public sealed class EventBinding<T> where T : struct, IEvent
    {
        private readonly Action<T> _onEvent;
        private readonly Action _onEventNoArgs;

        public EventBinding(Action<T> onEvent) => _onEvent = onEvent;
        public EventBinding(Action onEventNoArgs) => _onEventNoArgs = onEventNoArgs;

        internal void Invoke(T e)
        {
            _onEvent?.Invoke(e);
            _onEventNoArgs?.Invoke();
        }
    }
}
