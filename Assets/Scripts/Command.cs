
using UnityEngine;
public class Command<T> : ICommand<T>
{
    private  IReceiver<T> _receiver;

    public Command(IReceiver<T> receiver)
    {
        this._receiver = receiver;
    }
    public void Cancel()
    {
        this._receiver.CancelAction();
    }
    public void Execute(T value= default)
    {
        this._receiver.PerformAction(value);
    }
}
