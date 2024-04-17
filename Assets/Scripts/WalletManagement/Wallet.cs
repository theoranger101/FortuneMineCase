using System.Collections.Generic;
using Events;
using Roulette;
using Sirenix.OdinInspector;
using Toolkit.Singletons;

namespace WalletManagement
{
    public class Wallet : SingletonBehaviour<Wallet>
    {
        private Dictionary<RewardItem, int> RewardWallet = new();

        private void Awake()
        {
            if (!SetupInstance())
            {
                return;
            }

            GEM.AddListener<WalletEvent>(OnEarnReward, channel: (int)WalletEventType.Earn);
            // GEM.AddListener<WalletEvent>(OnSpendReward, channel:(int)WalletEventType.Spend);
        }

        [Button]
        public void SetupWallet(RewardItemCollection collection)
        {
            foreach (var rewardItemData in collection.Items)
            {
                RewardWallet.Add(rewardItemData, 0);
            }
        }

        private void OnEarnReward(WalletEvent evt)
        {
            RewardWallet[evt.RewardItem] += evt.Amount;
        }
    }
}