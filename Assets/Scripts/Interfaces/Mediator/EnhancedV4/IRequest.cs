using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;

namespace Assets.Scripts.Interfaces.Mediator.EnhancedV4
{
    public interface IRequest<T>
    {
        public IEnumerator Request(INotify<T> obsever);
    }
}
