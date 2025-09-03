using System;
using System.Threading.Tasks;
using UnityEngine;

public interface IReceiverAsync<T>
{
    Task<T> PerformAction(T value = default);
    Task<T> CancelAction();
}

public interface IReceiverBase<VALUE>
{
    public Type getType()
    {
        return typeof(VALUE);
    }
}

public interface IReceiverEnhancedAsync<TYPE, VALUE> : IReceiverBase<VALUE> where TYPE: MonoBehaviour
{
    Task<ActionExecuted<VALUE>> PerformAction(VALUE value = default);
    Task<ActionExecuted<VALUE>> CancelAction(VALUE value = default);
}
