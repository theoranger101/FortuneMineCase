using System.Collections.Generic;
using Events;
using RewardItemManagement;

namespace UI
{
    public enum UIEventType
    {
        RewardPopup = 0,
        WalletPopup = 1,
    }

    public class UIEvent : Event<UIEvent>
    {
        public RewardItem RewardItem;

        public Dictionary<RewardItem, int> WalletDictionary;

        // public Promise<bool> PopupPromise;

        public static UIEvent Get(RewardItem rewardItem)
        {
            var evt = GetPooledInternal();
            evt.RewardItem = rewardItem;

            return evt;
        }

        public static UIEvent Get(Dictionary<RewardItem, int> walletDict)
        {
            var evt = GetPooledInternal();
            evt.WalletDictionary = walletDict;

            return evt;
        }
    }
}