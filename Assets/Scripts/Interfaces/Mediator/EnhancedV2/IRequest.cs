using System.Collections.Generic;

namespace Assets.Scripts.Interfaces.Mediator.EnhancedV2
{
    public interface IRequest<T>: Base.IRequest
    {
        public IEnumerator<T> Request();
    }
}
