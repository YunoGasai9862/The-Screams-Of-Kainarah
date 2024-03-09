using System.Linq;
public class EventsHelper
{
    public EventsHelper() { }
    public UnityEventWOT GetCustomUnityEvent(EventStringMapper events, string animationEventName)
    {
        var eventFound = events.mappings.Where(e => e.eventIdentifier == animationEventName).FirstOrDefault().eventName;
        return eventFound;
    }

}