using UnityEngine.Events;

namespace Assets.Scripts.Interfaces
{
    public interface ICustomUnityAction
    {
        public void AddListener<T>(UnityAction<T> action) where T : GenericStateBundle;

        public UnityAction<T> GetAction<T>() where T : GenericStateBundle;

        public void Invoke<T>(T value) where T : GenericStateBundle;
    }
}
