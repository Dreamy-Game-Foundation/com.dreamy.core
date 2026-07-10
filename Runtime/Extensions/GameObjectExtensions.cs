using UnityEngine;

namespace Dreamy.Core
{
    public static class GameObjectExtensions
    {
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            if (!gameObject)
            {
                return;
            }

            gameObject.layer = layer;
            var transform = gameObject.transform;
            for (var i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetLayerRecursively(layer);
            }
        }

        public static void DestroyAllChildren(this GameObject gameObject)
        {
            if (gameObject)
            {
                gameObject.transform.DestroyChildren();
            }
        }

        public static void DestroyAllChildrenImmediate(this GameObject gameObject)
        {
            if (gameObject)
            {
                gameObject.transform.DestroyChildrenImmediate();
            }
        }

        public static void SetActiveIfChanged(this GameObject gameObject, bool isActive)
        {
            if (gameObject && gameObject.activeSelf != isActive)
            {
                gameObject.SetActive(isActive);
            }
        }
    }
}
