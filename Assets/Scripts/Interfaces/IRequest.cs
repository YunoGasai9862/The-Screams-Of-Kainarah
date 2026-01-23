using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Interfaces
{

    public interface IRequest
    {
        public IEnumerator Request();
    }

    //in case if we ever want to use <T> - great for signatures, etc
    public interface IRequest<T>: IRequest
    {
        public new IEnumerator<T> Request();
    }
}
