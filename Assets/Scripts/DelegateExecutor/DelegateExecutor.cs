using System.Threading.Tasks;
using UnityEngine;

public class DelegateExecutor: MonoBehaviour, IDelegateExecutor
{
    private void Start()
    {
        
    }

    public Task ExecuteDelegateMethod(IDeletegate delegateMethod)
    {
        delegateMethod.InvokeCustomMethod();

        return Task.CompletedTask;
    }
}