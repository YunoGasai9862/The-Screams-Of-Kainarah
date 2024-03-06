using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EventStringMapper: MonoBehaviour
{
    [Serializable]
    public class EventMappings
    {
        public string eventIdentifier;
        public UnityEvent eventName;
    }

   [SerializeField] public EventMappings[] mappings;
}