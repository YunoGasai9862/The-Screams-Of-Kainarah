using UnityEngine;
using UnityEngine.Events;

//with type <T>
public abstract class UnityEventWT<T> : MonoBehaviour, ICustomUnityEventWT<T> //extends the base class, but adds GetInstance functionality
{
    public abstract UnityEvent<T> GetInstance();
}