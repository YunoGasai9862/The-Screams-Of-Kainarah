using System.Collections.Generic;

namespace Assets.Scripts.Interfaces.Mediator.Enhanced
{
    public interface IRequest<T>
    {
        public IEnumerator<T> Request(INotify<T> obsever);
    }
}
