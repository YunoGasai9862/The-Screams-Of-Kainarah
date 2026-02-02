using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Interfaces.Mediator.Enhanced
{
    public interface IRequest<T>
    {
        public IEnumerator<T> Request(INotify<T> obsever);
    }
}
