using Events;

namespace Roulette
{
    public enum RouletteEventType
    {
        Spin = 0,
    }

    public class RouletteEvent : Event<RouletteEvent>
    {
        public static RouletteEvent Get()
        {
            var evt = GetPooledInternal();

            return evt;
        }
    }
}