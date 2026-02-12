using System.Collections;

namespace Assets.Scripts.Interfaces.Mediator.EnhancedV4
{
    public interface IRequest<T>
    {
        public IEnumerator Request(INotify<T> obsever);
    }
}
