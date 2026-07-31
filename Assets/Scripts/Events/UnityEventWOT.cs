using Assets.Scripts.BaseScene;
using System;
using UnityEngine;
using UnityEngine.Events;

//without type <T>
[Serializable]
public abstract class UnityEventWOT : MonoBehaviorScene, ICustomUnityEventWOT //extends the base class, but adds GetInstance functionality
{
    public abstract UnityEvent GetInstance();
}