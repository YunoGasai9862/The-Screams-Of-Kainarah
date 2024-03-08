using UnityEngine.Events;

public interface ICustomUnityEventWT<T>
{
   UnityEvent<T> GetInstance();
}