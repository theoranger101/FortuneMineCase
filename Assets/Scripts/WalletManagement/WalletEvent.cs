using Events;
using Roulette;

namespace WalletManagement
{
    public enum WalletEventType
    {
        Earn = 0,
        Spend = 1,
    }

    public class WalletEvent : Event<WalletEvent>
    {
        public RewardItem RewardItem;
        public int Amount;

        public static WalletEvent Get(RewardItem item, int amount)
        {
            var evt = GetPooledInternal();
            evt.RewardItem = item;
            evt.Amount = amount;

            return evt;
        }
    }
}