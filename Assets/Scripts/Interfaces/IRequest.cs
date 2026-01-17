using System.Threading.Tasks;

namespace Assets.Scripts.Interfaces
{

    public interface IRequest
    {
        public Task<object> Request();
    }

    public interface IRequest<T>
    {
        public Task<T> Request();
    }
}
