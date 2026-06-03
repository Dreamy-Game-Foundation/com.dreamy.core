using System.Collections.Generic;
using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>Token returned by BindableProperty.Register(). Call UnRegister() to stop listening.</summary>
    public interface IUnRegister
    {
        void UnRegister();
    }

    public static class UnRegisterExtensions
    {
        /// <summary>
        /// Automatically calls UnRegister() when the target GameObject is destroyed.
        /// </summary>
        public static IUnRegister UnRegisterOnDestroy(this IUnRegister unRegister, GameObject gameObject)
        {
            gameObject.GetOrAddComponent<UnRegisterOnDestroyTrigger>().Add(unRegister);
            return unRegister;
        }
    }

    /// <summary>Internal MonoBehaviour that holds IUnRegister tokens and cleans them on Destroy.</summary>
    [AddComponentMenu("")]
    public sealed class UnRegisterOnDestroyTrigger : MonoBehaviour
    {
        private readonly List<IUnRegister> _unRegisters = new();

        internal void Add(IUnRegister unRegister) => _unRegisters.Add(unRegister);

        private void OnDestroy()
        {
            foreach (var ur in _unRegisters)
                ur.UnRegister();
            _unRegisters.Clear();
        }
    }
}
