
using System;
using System.Threading.Tasks;
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

public class CommandAsyncEnhanced<T, Z> : ICommandAsyncEnhanced<T, Z> where T: MonoBehaviour
{
    private IReceiverEnhancedAsync<T, Z> _receiver;

    public CommandAsyncEnhanced(IReceiverEnhancedAsync<T, Z> receiver)
    {
        _receiver = receiver;
    }
    public async Task<ActionExecuted<Z>> Cancel(Z value = default)
    {
        return await _receiver.CancelAction();
    }
    public async Task<ActionExecuted<Z>> Execute(Z value = default)
    {
        return await _receiver.PerformAction(value);
    }

    public Type GetExecutingType()
    {
        return typeof(T);
    }
}

