using System;
using UnityEngine.Events;

//without type <T>
[Serializable]
public abstract class UnityEventWOT : UnityEvent //extends the base class, but adds GetInstance functionality
{
    public abstract UnityEventWOT GetInstance();
}