using System.Threading.Tasks;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;

namespace Assets.Scripts.Interfaces.Mediator.EnhancedV3
{
    public interface IRequest<T>
    {
        public Task<T> Request(INotify<T> obsever);
    }
}
