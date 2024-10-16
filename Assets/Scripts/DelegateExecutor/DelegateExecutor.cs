using System.Collections.Generic;
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

    private async Task ExecuteDelegates(PreloadEntity[] preloadEntities)
    {
        await ExecuteDelegatesForScriptableObjects(preloadEntities.Where(pe => pe.EntitySO != null).ToList());

        await ExecuteDelegatesForGameObjects(preloadEntities.Where(pe => pe.EntityMB != null).ToList());
    }

    private Task ExecuteDelegatesForScriptableObjects(List<PreloadEntity> preloadEntities)
    {
        //for now nothing
        return Task.CompletedTask;
    }

    private async Task ExecuteDelegatesForGameObjects(List<PreloadEntity> preloadEntities)
    {
        foreach (PreloadEntity preloadEntity in preloadEntities)
        {

            IDelegate delegateObject = preloadEntity.EntityMB.gameObject.GetComponent<IDelegate>();

            Debug.Log($"Found Delegate: {delegateObject}");

            await ExecuteDelegateMethod(delegateObject);
     
        }

    }

    private async void PreloadEntitiesEventListener(PreloadEntity[] preloadEntities)
    {
        Debug.Log("Executing Preload Entities Event Listener");

        await ExecuteDelegates(preloadEntities);
    }
    
}