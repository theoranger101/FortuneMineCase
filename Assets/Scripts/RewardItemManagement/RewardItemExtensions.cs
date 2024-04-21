using Events;

namespace RewardItemManagement
{
    public static class RewardItemExtensions
    {
        public static RewardItemRuntimeData RuntimeData(this RewardItem item)
        {
            using var evt = RewardItemEvent.Get(item).SendGlobal((int)RewardItemEventType.RequestData);
            return evt.RuntimeData;
        }
    }
}