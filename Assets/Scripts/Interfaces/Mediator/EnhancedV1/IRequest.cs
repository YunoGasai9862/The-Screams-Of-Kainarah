using System.Collections;

namespace Assets.Scripts.Interfaces.Mediator.EnhancedV1
{
    public interface IRequest: Base.IRequest
    {
        public IEnumerator Request();
    }

    public interface IRequest<T>: Base.IRequest
    {
        public IEnumerator Request();
    }
}
