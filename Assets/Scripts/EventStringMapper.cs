using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EventStringMapperScriptableObjectEntity", menuName = "Event String Mapper")]
public class EventStringMapper: ScriptableObject
{
    [Serializable]
    public class EventMappingsWithoutType
    {
        public string eventIdentifier;
        public UnityEventWOT eventNamWithoutType;
    }

    [Serializable]
    public class EventMappingsWithType<T>
    {
        public string eventIdentifier;
        public UnityEventWT<T> eventNameWithType;
    }

    //without Type
    public EventMappingsWithoutType[] mappingsWOT;

    //with Type
    [Header("Unity Event (Bool)")]
    public EventMappingsWithType<bool>[] mappingWTBool;
    [Header("Unity Event (String)")]
    public EventMappingsWithType<string>[] mappingWTString;
    [Header("Unity Event (Float)")]
    public EventMappingsWithType<float>[] mappingWTFloat;
    [Header("Unity Event (Double)")]
    public EventMappingsWithType<double>[] mappingWTDouble;

}