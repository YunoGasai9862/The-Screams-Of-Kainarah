using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Actions
{
    public abstract class UnityAction : MonoBehaviour, ICustomUnityAction
    {
        public abstract void AddListener<T>(UnityAction<dynamic> action);

        public abstract UnityAction<dynamic> GetAction<T>();

        public abstract void Invoke<T>(dynamic value);
    }
}
