using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

public interface IAsyncCoroutine<T>
{   
    public Task ExecuteAsyncCoroutine(IAsyncEnumerator<T> asyncCoroutine);
}