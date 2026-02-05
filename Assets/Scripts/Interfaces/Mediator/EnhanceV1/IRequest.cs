using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Interfaces.Mediator.EnhancedV1
{
    public interface IRequest<T>
    {
        public IEnumerator<T> Request();
    }
}
