using UnityEngine.Events;

namespace Assets.Scripts.Interfaces
{
    public interface ICustomUnityAction
    {
        public abstract UnityAction<dynamic> GetAction<T>();
        public abstract void AddListener<T>(UnityAction<dynamic> action);
        public abstract void Invoke<T>(dynamic value);
    }
}
