using System.Threading.Tasks;
using UnityEngine;

public interface IReceiverAsync<T>
{
    Task<T> PerformAction(T value = default);
    Task<T> CancelAction();
}

public interface IReceiverEnhancedAsync<TYPE, VALUE> where TYPE: MonoBehaviour
{
    Task<ActionExecuted<VALUE>> PerformAction(VALUE value = default);
    Task<ActionExecuted<VALUE>> CancelAction();
}
