using System.Threading.Tasks;

public interface IMediator
{
    public abstract Task NotifyManager(NotifyPackage notifyPackage);
}