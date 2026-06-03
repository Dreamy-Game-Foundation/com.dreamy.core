using System;
using System.Collections.Generic;

namespace Dreamy.Core
{
    /// <summary>
    /// Reactive property. Notifies listeners when Value changes.
    /// Use Register() to listen and store the returned IUnRegister token for cleanup.
    /// </summary>
    /// <example>
    /// BindableProperty&lt;int&gt; _coins = new(0);
    ///
    /// // Subscribe + auto-cleanup when gameObject is destroyed
    /// _coins.RegisterWithInitValue(v => coinsText.text = v.ToString())
    ///       .UnRegisterOnDestroy(gameObject);
    ///
    /// // Change value (fires listeners)
    /// _coins.Value += 10;
    ///
    /// // Change without firing
    /// _coins.SetValueSilently(0);
    /// </example>
    public class BindableProperty<T>
    {
        private T _value;
        private readonly List<Action<T>> _listeners = new();

        public BindableProperty(T defaultValue = default) => _value = defaultValue;

        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value)) return;
                _value = value;
                NotifyListeners();
            }
        }

        /// <summary>Sets value without firing listeners.</summary>
        public void SetValueSilently(T value) => _value = value;

        /// <summary>Register a listener. Returns an IUnRegister token for cleanup.</summary>
        public IUnRegister Register(Action<T> listener)
        {
            _listeners.Add(listener);
            return new BindableUnRegister(this, listener);
        }

        /// <summary>Register and immediately invoke listener with current value.</summary>
        public IUnRegister RegisterWithInitValue(Action<T> listener)
        {
            listener(_value);
            return Register(listener);
        }

        public void UnRegister(Action<T> listener) => _listeners.Remove(listener);

        public void ClearListeners() => _listeners.Clear();

        private void NotifyListeners()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                try { _listeners[i](_value); }
                catch (Exception e) { UnityEngine.Debug.LogException(e); }
            }
        }

        private sealed class BindableUnRegister : IUnRegister
        {
            private readonly BindableProperty<T> _property;
            private readonly Action<T> _listener;

            public BindableUnRegister(BindableProperty<T> property, Action<T> listener)
            {
                _property = property;
                _listener = listener;
            }

            public void UnRegister() => _property.UnRegister(_listener);
        }
    }
}
