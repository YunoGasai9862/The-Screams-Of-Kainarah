
using UnityEngine;
public class Command<T> : ICommand<T>
{
    private  IReceiver<T> _receiver;

    public Command(IReceiver<T> receiver)
    {
        this._receiver = receiver;
    }
    public void Cancel(T value= default)
    {
        this._receiver.CancelAction();
    }
    public void Execute(T value= default)
    {
        this._receiver.PerformAction(value);
    }
}

public class Command<T, Z> : ICommand<T, Z>
{
    private IReceiver<T, Z> _receiver;

    public Command(IReceiver<T, Z> receiver)
    {
        this._receiver = receiver;
    }
    public Z Cancel(T value = default)
    {
        return this._receiver.CancelAction();
    }
    public Z Execute(T value = default)
    {
        return this._receiver.PerformAction(value);
    }
}

