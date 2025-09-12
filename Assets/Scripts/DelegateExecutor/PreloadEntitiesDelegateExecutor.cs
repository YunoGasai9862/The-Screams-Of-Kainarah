using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PreloadEntitiesDelegateExecutor: MonoBehaviour, IDelegateExecutor
{

    [SerializeField]
    PreloadedEntitiesEvent preloadedEntitiesEvent;

    private void Start()
    {
        preloadedEntitiesEvent.AddListener(PreloadEntitiesEventListener);
    }

    public Task ExecuteDelegateMethod(IDelegate delegateMethod)
    {
        delegateMethod.InvokeCustomMethod();

        return Task.CompletedTask;
    }

    private async Task ExecuteDelegates(List<UnityEngine.Object> preloadedEntities)
    {
        await ExecuteDelegatesForScriptableObjects(preloadedEntities.Where(pe => pe is ScriptableObject).Cast<ScriptableObject>().ToList()); //casting is needed

        await ExecuteDelegatesForGameObjects(preloadedEntities.Where(pe => pe is GameObject).Cast<GameObject>().ToList());
    }

    private async Task ExecuteDelegatesForScriptableObjects(List<ScriptableObject> preloadEntities)
    {
        foreach(ScriptableObject scriptableObject in preloadEntities)
        {
            Debug.Log($"ExecuteDelegatesForScriptableObjects - {scriptableObject}");

            if (scriptableObject is IDelegate)
            {
                Debug.Log($"Implements IDelegate - {scriptableObject}");

                await ExecuteDelegateMethod((IDelegate)scriptableObject);
            }
        }
    }

    private async Task ExecuteDelegatesForGameObjects(List<GameObject> preloadedEntities)
    {
        foreach (GameObject preloadEntity in preloadedEntities)
        {
            IDelegate delegateObject;

            if (preloadEntity.gameObject.TryGetComponent(out delegateObject))
            {
                await ExecuteDelegateMethod(delegateObject);
            }
        }
    }

    private async void PreloadEntitiesEventListener(List<UnityEngine.Object> preloadedEntities)
    {
        await ExecuteDelegates(preloadedEntities);
    }
    
}