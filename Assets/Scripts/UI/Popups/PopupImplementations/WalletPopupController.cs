using System.Collections.Generic;
using Events;
using RewardItemManagement;
using UnityEngine;

namespace UI
{
    public class WalletPopupController : PopupController
    {
        public GameObject RewardItemUIPrefab;

        public Transform Row1, Row2;

        public List<RewardItemWalletUI> RewardItemWalletUIs = new();

        protected override void Awake()
        {
            base.Awake();
            GEM.AddListener<UIEvent>(OnPopupEvent, channel: (int)UIEventType.WalletPopup);
        }

        private void OnDisable()
        {
            GEM.RemoveListener<UIEvent>(OnPopupEvent, channel: (int)UIEventType.WalletPopup);
        }

        public override void Initialize(object arg)
        {
            var walletDict = (Dictionary<RewardItem, int>) arg;

            if (walletDict == null)
            {
                base.Initialize(arg);
                return;
            }
            
            var halfCount = walletDict.Count / 2;
            var count = 0;
            
            foreach (var rewardItemPair in walletDict)
            {
                if (count < RewardItemWalletUIs.Count)
                {
                    RewardItemWalletUIs[count].Setup(rewardItemPair.Key, rewardItemPair.Value);
                    count++;
                    
                    continue;
                }
                
                var newUI = Instantiate(RewardItemUIPrefab, count < halfCount ? Row1 : Row2);
                var newRwdWalletUi = newUI.GetComponent<RewardItemWalletUI>();

                newRwdWalletUi.Setup(rewardItemPair.Key, rewardItemPair.Value);

                RewardItemWalletUIs.Add(newRwdWalletUi);
                count++;
            }

            base.Initialize(arg);
        }

        public override void OnPopupEvent(UIEvent evt)
        {
            Initialize(evt.WalletDictionary);
        }
    }
}