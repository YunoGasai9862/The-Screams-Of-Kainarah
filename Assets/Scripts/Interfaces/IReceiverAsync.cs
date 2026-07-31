using Assets.Scripts.BaseScene;
using System;
using System.Threading.Tasks;
using UnityEngine;

public interface IReceiverAsync<T> : IReceiverBase<T>
{
    Task<T> PerformAction(T value = default);
    Task<T> CancelAction();
}

public interface IReceiverBase<VALUE>
{
    public Type GetType()
    {
        return typeof(VALUE);
    }
}

public interface IReceiverEnhancedAsync<TYPE, VALUE> : IReceiverBase<VALUE> where TYPE: MonoBehaviorScene
{
    Task<ActionExecuted> PerformAction(VALUE value = default);
    Task<ActionExecuted> CancelAction(VALUE value = default);
}
