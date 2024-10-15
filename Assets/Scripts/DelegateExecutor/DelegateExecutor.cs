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
        foreach (PreloadEntity preloadEntity in preloadEntities)
        {
            //for now only for game objects
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