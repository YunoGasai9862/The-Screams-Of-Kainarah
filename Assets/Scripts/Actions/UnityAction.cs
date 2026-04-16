using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Actions
{
    public abstract class UnityAction : MonoBehaviour, ICustomUnityAction
    {
        public abstract void AddListener<T>(UnityAction<GenericStateBundle<T>> action) where T : IStateBundle;

        public abstract UnityAction<GenericStateBundle<T>> GetAction<T>() where T : IStateBundle;

        public abstract void Invoke<T>(GenericStateBundle<T> value) where T : IStateBundle;
    }
}
