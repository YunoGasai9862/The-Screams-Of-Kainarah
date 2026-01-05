using System.Threading.Tasks;

namespace Assets.Scripts.Interfaces
{
    public interface IRequest<T>
    {
        public Task<Context<T>> Request();
    }
}
