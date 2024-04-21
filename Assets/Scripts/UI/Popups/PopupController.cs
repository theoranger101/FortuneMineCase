using Conditionals;
using UnityEngine;

namespace UI
{
    public abstract class PopupController : MonoBehaviour, IPopup
    {
        public CanvasGroup CanvasGroup;
        public RectTransform Panel;

        protected virtual void Awake()
        {
            Toggle(false);
        }

        public virtual void Initialize(object arg)
        {
            Toggle(true);

            Conditional.Wait(1f)
                .Do(() => { Toggle(false); });
        }
        public abstract void OnPopupEvent(UIEvent evt);

        public virtual void Toggle(bool visible)
        {
            CanvasGroup.Toggle(visible, 0.2f);
            Panel.Toggle(visible, 0.2f);
        }
    }
}