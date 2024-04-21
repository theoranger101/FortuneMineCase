using System;
using Events;
using RewardItemManagement;
using UnityEngine.UI;

namespace UI
{
    public class RewardPopupController : PopupController
    {
        public Image RewardIcon;

        protected override void Awake()
        {
            base.Awake();
            GEM.AddListener<UIEvent>(OnPopupEvent, channel: (int)UIEventType.RewardPopup);
        }

        private void OnDisable()
        {
            GEM.RemoveListener<UIEvent>(OnPopupEvent, channel: (int)UIEventType.RewardPopup);
        }

        public override void Initialize(object arg)
        {
            var item = (RewardItem)arg;

            var promise = item.RuntimeData().GetIcon();
            promise.OnResultT += (success, sprite) =>
            {
                if (!success)
                    throw new Exception();

                RewardIcon.sprite = sprite;
            };

           base.Initialize(arg);
        }

        public override void OnPopupEvent(UIEvent evt)
        {
            Initialize(evt.RewardItem);
        }
    }
}