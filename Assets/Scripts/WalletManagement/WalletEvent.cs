using System.Collections.Generic;
using Events;
using RewardItemManagement;
using Toolkit.Variables.VariableImplementations;

namespace WalletManagement
{
    public enum WalletEventType
    {
        Earn = 0,
        Spend = 1,
        GetValue = 3,
        GetAllValues = 4,
    }

    public class WalletEvent : Event<WalletEvent>
    {
        public RewardItem RewardItem;
        public int Amount;

        public Dictionary<RewardItem, int> AllValues;

        public static WalletEvent Get(RewardItem item, int amount)
        {
            var evt = GetPooledInternal();
            evt.RewardItem = item;
            evt.Amount = amount;

            return evt;
        }

        public static WalletEvent Get(RewardItem item)
        {
            var evt = GetPooledInternal();
            evt.RewardItem = item;

            return evt;
        }
        
        public static WalletEvent Get()
        {
            var evt = GetPooledInternal();
            return evt;
        }
    }
}