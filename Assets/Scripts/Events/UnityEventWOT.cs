using System;
using UnityEngine;
using UnityEngine.Events;

//without type <T>
[Serializable]
public abstract class UnityEventWOT : MonoBehaviour, ICustomUnityEventWOT //extends the base class, but adds GetInstance functionality
{
    public abstract UnityEvent GetInstance();
}