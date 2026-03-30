using Annotations.Enums;
using System;
using UnityEngine;

[Asset(Asset.SCRIPTABLE_OBJECT, "EventStringMapper", InstantiationOrder = 11)]
[CreateAssetMenu(fileName = "EventStringMapperScriptableObjectEntity", menuName = "Event String Mapper")]
public class EventStringMapper: ScriptableObject
{
    [Serializable]
    public class EventMappingsWithoutType
    {
        public string eventIdentifier;
        public UnityEventWOT eventNameWithoutType;
    }

    public EventMappingsWithoutType[] mappingsWOT;

    [Serializable]
    public class EventMappingsWithType<T>
    {
        public string eventIdentifier;
        public UnityEventWTAsync<T> eventNameWithType;
    }

    [Header("Unity Event (Bool)")]
    public EventMappingsWithType<bool>[] mappingWTBool;
    [Header("Unity Event (String)")]
    public EventMappingsWithType<string>[] mappingWTString;
    [Header("Unity Event (Float)")]
    public EventMappingsWithType<float>[] mappingWTFloat;
    [Header("Unity Event (Double)")]
    public EventMappingsWithType<double>[] mappingWTDouble;

}