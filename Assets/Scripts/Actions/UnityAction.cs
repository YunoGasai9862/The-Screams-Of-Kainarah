using Assets.Scripts.Interfaces;
using UnityEngine.Events;

namespace Assets.Scripts.Actions
{
    public abstract class UnityAction : GetBaseScene().MonoBehaviorScene, ICustomUnityAction
    {
        public abstract void AddListener<T>(UnityAction<T> action) where T : GenericStateBundle;

        public abstract UnityAction<T> GetAction<T>() where T : GenericStateBundle;

        public abstract void Invoke<T>(T value) where T : GenericStateBundle;
    }
}
