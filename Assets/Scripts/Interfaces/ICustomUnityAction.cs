using UnityEngine.Events;

namespace Assets.Scripts.Interfaces
{
    public interface ICustomUnityAction
    {
        public void AddListener<T>(UnityAction<GenericStateBundle<T>> action) where T : IStateBundle;

        public UnityAction<GenericStateBundle<T>> GetAction<T>() where T : IStateBundle;

        public void Invoke<T>(GenericStateBundle<T> value) where T : IStateBundle;
    }
}
