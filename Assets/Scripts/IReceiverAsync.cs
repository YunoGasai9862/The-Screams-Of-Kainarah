using System.Threading.Tasks;
using UnityEngine;

public interface IReceiverAsync<T>
{
    Task<T> PerformAction(T value = default);
    Task<T> CancelAction();
}
