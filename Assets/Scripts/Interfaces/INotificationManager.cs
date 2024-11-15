using System;
using System.Threading.Tasks;

public interface INotificationManager
{
   public abstract Task RelayNotification();

   public Type EntityType();
}