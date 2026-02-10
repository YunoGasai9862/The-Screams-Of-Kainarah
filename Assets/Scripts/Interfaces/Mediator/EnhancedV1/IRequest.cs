using System.Collections;

namespace Assets.Scripts.Interfaces.Mediator.EnhancedV1
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
