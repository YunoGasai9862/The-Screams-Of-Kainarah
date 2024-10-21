using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface INotifcation
{
    public Task NotifyEntity(List<IListenerEntity> notifyingEntities);

    public Task UpdateNotifyList(string tag, bool isActive);
}