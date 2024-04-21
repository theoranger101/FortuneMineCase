using System.Collections.Generic;
using Events;
using RewardItemManagement;
using RouletteManagement;
using Toolkit.Singletons;
using Toolkit.Variables.VariableImplementations;
using UnityEngine;

namespace WalletManagement
{
    public class Wallet : SingletonBehaviour<Wallet>
    {
        private Dictionary<RewardItem, int> RewardWallet = new();

        private void Awake()
        {
            if(!SetupInstance())
                return;
            
            GEM.AddListener<RouletteEvent>(OnRouletteSetupComplete,
                channel: (int)RouletteEventType.SetupComplete);

            GEM.AddListener<WalletEvent>(OnEarnReward, channel: (int)WalletEventType.Earn);
            GEM.AddListener<WalletEvent>(OnGetValue, channel: (int)WalletEventType.GetValue);
            GEM.AddListener<WalletEvent>(OnGetAllValues, channel: (int)WalletEventType.GetAllValues);
            // GEM.AddListener<WalletEvent>(OnSpendReward, channel:(int)WalletEventType.Spend);
        }

        private void OnDisable()
        {
            GEM.RemoveListener<WalletEvent>(OnEarnReward, channel: (int)WalletEventType.Earn);
            GEM.RemoveListener<WalletEvent>(OnGetValue, channel: (int)WalletEventType.GetValue);
            GEM.RemoveListener<WalletEvent>(OnGetAllValues, channel: (int)WalletEventType.GetAllValues);
        }

        private void OnRouletteSetupComplete(RouletteEvent evt)
        {
            SetupWallet(evt.Collection);
        }

        private void SetupWallet(RewardItemCollection collection)
        {
            foreach (var rewardItemData in collection.Items)
            {
                if(RewardWallet.ContainsKey(rewardItemData))
                    continue;

                var intVariable = ScriptableObject.CreateInstance<IntVariable>();
                RewardWallet.Add(rewardItemData, intVariable);
            }
        }

        private void OnEarnReward(WalletEvent evt)
        {
            RewardWallet[evt.RewardItem] += evt.Amount;
        }

        private void OnGetValue(WalletEvent evt)
        {
            evt.Amount = RewardWallet[evt.RewardItem];
        }
        
        private void OnGetAllValues(WalletEvent evt)
        {
            evt.AllValues = RewardWallet;
        }
    }
}