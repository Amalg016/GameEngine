
namespace GameEngine.observers.events
{
    public class Event
    {
     public EventType Type;
        public Event(EventType eventType)
        {
            Type = eventType;
        }  
        public Event()
        {
            this.Type = EventType.UserEvent;
        }

    }
}
