using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;

namespace Assets.Scripts.Interfaces.Mediator.EnhancedV4
{
    public interface IRequest<T>: Base.IRequest
    {
        public IEnumerator Request(INotify<T> obsever);
    }
}
