using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EventStringMapperScriptableObjectEntity", menuName = "Event String Mapper")]
public class EventStringMapper: ScriptableObject
{
    [Serializable]
    public class EventMappings
    {
        public string eventIdentifier;
        public UnityEventWOT eventName;
    }

   public EventMappings[] mappings;
}