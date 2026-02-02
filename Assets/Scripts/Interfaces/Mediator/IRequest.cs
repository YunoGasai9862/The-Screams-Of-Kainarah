using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Interfaces.Mediator
{
    public interface IRequest
    {
        public IEnumerator Request();
    }

    public interface IRequest<T>
    {
        public IEnumerator<T> Request();
    }
}
