using UnityEngine;

namespace Dreamy.Core
{
    public static class RectTransformExtensions
    {
        public static void StretchFull(this RectTransform rectTransform)
        {
            if (!rectTransform)
            {
                return;
            }

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        public static void SetAnchoredPositionX(this RectTransform rectTransform, float x)
        {
            if (!rectTransform)
            {
                return;
            }

            var position = rectTransform.anchoredPosition;
            position.x = x;
            rectTransform.anchoredPosition = position;
        }

        public static void SetAnchoredPositionY(this RectTransform rectTransform, float y)
        {
            if (!rectTransform)
            {
                return;
            }

            var position = rectTransform.anchoredPosition;
            position.y = y;
            rectTransform.anchoredPosition = position;
        }

        public static void SetSizeDeltaX(this RectTransform rectTransform, float x)
        {
            if (!rectTransform)
            {
                return;
            }

            var size = rectTransform.sizeDelta;
            size.x = x;
            rectTransform.sizeDelta = size;
        }

        public static void SetSizeDeltaY(this RectTransform rectTransform, float y)
        {
            if (!rectTransform)
            {
                return;
            }

            var size = rectTransform.sizeDelta;
            size.y = y;
            rectTransform.sizeDelta = size;
        }
    }
}
