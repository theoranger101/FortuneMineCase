using UnityEngine;
using DG.Tweening;

namespace UI
{
    public static class UIExtensions
    {
        public static void Toggle(this CanvasGroup canvasGroup, bool visible, float duration)
        {
            canvasGroup.DOFade(visible ? 1 : 0, duration);
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable = visible;
        }

        public static void DoScaleAnim(this RectTransform rectTransform, Vector3 scale, float duration,
            Ease ease = Ease.InOutBack)
        {
            rectTransform.DOScale(scale, duration).SetEase(Ease.InOutBounce);
        }

        public static void Shrink(this RectTransform rectTransform, float duration,
            Ease ease = Ease.InOutBack)
        {
            rectTransform.DOScale(Vector3.zero, duration).SetEase(Ease.InOutBounce);
        }

        public static void Expand(this RectTransform rectTransform, float duration,
            Ease ease = Ease.InOutBack)
        {
            rectTransform.DOScale(Vector3.one, duration).SetEase(Ease.InOutBounce);
        }

        public static void Toggle(this RectTransform rectTransform, bool visible, float duration)
        {
            if (visible)
            {
                rectTransform.Expand(duration);
            }
            else
            {
                rectTransform.Shrink(duration);
            }
        }
    }
}