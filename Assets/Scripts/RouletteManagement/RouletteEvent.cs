using Events;
using RewardItemManagement;

namespace RouletteManagement
{
    public enum RouletteEventType
    {
        SetupComplete = 0,
        Spin = 1,
        SpinSequenceEnd = 2,
        AllSlotsSelected = 3,
    }

    public class RouletteEvent : Event<RouletteEvent>
    {
        public RewardItemCollection Collection;

        public static RouletteEvent Get()
        {
            var evt = GetPooledInternal();

            return evt;
        }

        public static RouletteEvent Get(RewardItemCollection collection)
        {
            var evt = GetPooledInternal();
            evt.Collection = collection;

            return evt;
        }
    }
}