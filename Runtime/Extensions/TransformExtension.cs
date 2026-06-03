using UnityEngine;

namespace Dreamy.Core
{
    public static class TransformExtension
    {
        public static void ResetLocal(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        public static void SetParentAndReset(this Transform t, Transform parent)
        {
            t.SetParent(parent);
            t.ResetLocal();
        }

        public static void SetActiveChildren(this Transform t, bool active)
        {
            for (int i = 0; i < t.childCount; i++)
                t.GetChild(i).gameObject.SetActive(active);
        }

        public static void DestroyChildren(this Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
                Object.Destroy(t.GetChild(i).gameObject);
        }

        public static void DestroyChildrenImmediate(this Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(t.GetChild(i).gameObject);
        }
    }
}
