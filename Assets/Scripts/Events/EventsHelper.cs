using UnityEngine;
using System.Linq;
public class EventsHelper
{
    public EventsHelper() { }
    public UnityEventWOT GetCustomUnityEvent(EventStringMapper events, string animationEventName)
    {
        var eventFound = events.mappingsWOT.Where(e => e.eventIdentifier == animationEventName).FirstOrDefault().eventNameWithoutType;
        return eventFound;
    }
    public UnityEventWT<bool> GetCustomUnityEventWithType(EventStringMapper events, string animationEventName)
    {
        var eventFound = events.mappingWTBool.Where(e => e.eventIdentifier == animationEventName).FirstOrDefault().eventNameWithType;
        return eventFound;
    }
}