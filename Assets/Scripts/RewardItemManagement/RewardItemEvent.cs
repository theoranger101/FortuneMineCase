using Events;

namespace RewardItemManagement
{
    public enum RewardItemEventType
    {
        RequestData = 0,
        RequestItemWithId = 1,
    }

    public class RewardItemEvent : Event<RewardItemEvent>
    {
        public int Id;
        public RewardItem RewardItem;
        public RewardItemRuntimeData RuntimeData;

        public static RewardItemEvent Get(RewardItem rewardItem)
        {
            var evt = GetPooledInternal();
            evt.RewardItem = rewardItem;

            return evt;
        }

        public static RewardItemEvent Get(int id)
        {
            var evt = GetPooledInternal();
            evt.Id = id;

            return evt;
        }
    }
}