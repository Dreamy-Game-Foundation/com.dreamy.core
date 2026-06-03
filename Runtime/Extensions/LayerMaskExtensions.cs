using UnityEngine;

namespace Dreamy.Core
{
    public static class LayerMaskExtensions
    {
        /// <summary>Returns true if <paramref name="layerIndex"/> is included in this <see cref="LayerMask"/>.</summary>
        public static bool Contains(this LayerMask mask, int layerIndex) =>
            mask == (mask | (1 << layerIndex));
    }
}
