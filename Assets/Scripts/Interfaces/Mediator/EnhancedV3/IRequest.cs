using System.Collections.Generic;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;

namespace Assets.Scripts.Interfaces.Mediator.EnhancedV3
{
    public interface IRequest<T>
    {
        public IEnumerator<T> Request(INotify<T> obsever);
    }
}
