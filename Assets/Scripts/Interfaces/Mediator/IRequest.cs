using System.Collections;

namespace Assets.Scripts.Interfaces.Mediator
{
    public interface IRequest
    {
        public IEnumerator Request();
    }

    public interface IRequest<T>
    {
        public IEnumerator Request();
    }
}
