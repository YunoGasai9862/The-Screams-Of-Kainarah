using System.Collections.Generic;

namespace Assets.Scripts.Interfaces.Mediator.EnhancedV2
{
    public interface IRequest<T>
    {
        public IEnumerator<T> Request(INotify<T> obsever);
    }
}
