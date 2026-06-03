using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dreamy.Core
{
    public static class FadeExtensions
    {
        public static async UniTask FadeInAsync(this CanvasGroup cg, float duration)
        {
            cg.alpha = 0f;
            cg.gameObject.SetActive(true);
            await cg.FadeAsync(0f, 1f, duration);
        }

        public static async UniTask FadeOutAsync(this CanvasGroup cg, float duration, bool disableOnComplete = true)
        {
            await cg.FadeAsync(cg.alpha, 0f, duration);
            if (disableOnComplete)
                cg.gameObject.SetActive(false);
        }

        public static async UniTask FadeAsync(this CanvasGroup cg, float from, float to, float duration)
        {
            if (duration <= 0f)
            {
                cg.alpha = to;
                return;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
                await UniTask.NextFrame();
            }
            cg.alpha = to;
        }

        public static void SetVisible(this CanvasGroup cg, bool visible, bool interactable = true)
        {
            cg.alpha = visible ? 1f : 0f;
            cg.interactable = visible && interactable;
            cg.blocksRaycasts = visible;
        }
    }
}
