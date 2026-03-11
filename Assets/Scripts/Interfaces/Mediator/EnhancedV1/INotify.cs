using System.Collections;

namespace Assets.Scripts.Interfaces.Mediator.EnhancedV1
{
    public interface INotify: Base.INotify
    {
        public IEnumerator Notify(object value);
    }
    public interface INotify<T>: Base.INotify
    {
        public IEnumerator Notify(T value);
    }
}
