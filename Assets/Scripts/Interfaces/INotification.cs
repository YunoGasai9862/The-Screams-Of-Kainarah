using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface INotification
{
    public Task NotifyEntity(List<IListenerEntity> notifyingEntities);

    public Task PingNotificationManager(NotifierEntity notifierEntity);
}