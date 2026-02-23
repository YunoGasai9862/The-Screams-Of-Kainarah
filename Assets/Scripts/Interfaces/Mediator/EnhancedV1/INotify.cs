using System.Collections;

namespace Assets.Scripts.Interfaces.Mediator.EnhancedV1
{
    public interface INotify
    {
        public IEnumerator Notify(object value);
    }
    public interface INotify<T>
    {
        public IEnumerator Notify(T value);
    }
}
