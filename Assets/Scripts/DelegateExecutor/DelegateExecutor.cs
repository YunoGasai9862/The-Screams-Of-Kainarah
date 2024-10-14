using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DelegateExecutor: MonoBehaviour, IDelegateExecutor
{
    [SerializeField]
    PreloadEntitiesEvent preloadEntitiesEvent;

    private void Start()
    {
        preloadEntitiesEvent.AddListener(PreloadEntitiesEventListener);
    }

    public Task ExecuteDelegateMethod(IDelegate delegateMethod)
    {
        delegateMethod.InvokeCustomMethod();

        return Task.CompletedTask;
    }

    private Task ExecuteDelegates(PreloadEntity[] preloadEntities)
    {
        foreach (PreloadEntity preloadEntity in preloadEntities)
        {
            //for now only for game objects
            bool idelegateExists = preloadEntity.EntityMB.gameObject.GetComponents<IDelegate>().Any();

            // if more than implemented, throw exception error due to ambiguity, for preloading we only need one

        }

        return Task.CompletedTask;
    }

    private async void PreloadEntitiesEventListener(PreloadEntity[] preloadEntities)
    {
        await ExecuteDelegates(preloadEntities);
    }
    
}