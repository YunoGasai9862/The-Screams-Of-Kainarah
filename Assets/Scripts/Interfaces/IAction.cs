
using System;
using System.Threading.Tasks;
public interface IAction
{
    public abstract Task ExecuteAction<T>(Action<T> Action, T value);
}
