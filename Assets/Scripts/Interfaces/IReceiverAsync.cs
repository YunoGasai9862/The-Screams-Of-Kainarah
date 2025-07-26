using System.Threading.Tasks;
using UnityEngine;

public interface IReceiverAsync<T>
{
    Task<T> PerformAction(T value = default);
    Task<T> CancelAction();
}

public interface IReceiverEnhancedAsync<IDENTIFIER, VALUE>
{
    VALUE PerformAction(VALUE value = default);

    VALUE CancelAction();
}
