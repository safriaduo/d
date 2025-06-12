// EventBus Manager to handle all buses
namespace Dawnshard.Network
{
    public class EventBusManager
    {
        public EventBus<int, IGameEvent> ZoneEventBus { get; private set; }

        public EventBus<string, IGameEvent> PlayerEventBus { get; private set; }

        public EventBus<int, IGameEvent> CardEventBus { get; private set; }

        public IGameInteractor GameInteractor { get;  set; }

        public EventBusManager()
        {
            ZoneEventBus = new();
            PlayerEventBus = new();
            CardEventBus = new();
        }
    }
}