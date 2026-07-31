using Assets.Scripts.BaseScene;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PreloadEntitiesDelegateExecutor: MonoBehaviorScene, IDelegateExecutor
{
    [SerializeField]
    PreloadedEntitiesEvent preloadedEntitiesEvent;

    private void Start()
    {
        preloadedEntitiesEvent.AddListener(PreloadEntitiesEventListener);
    }

    public async Task ExecuteDelegates(List<UnityEngine.Object> preloadedEntities)
    {
        await ExecuteDelegatesForScriptableObjects(preloadedEntities.Where(pe => pe is ScriptableObject).Cast<ScriptableObject>().ToList()); //casting is needed

        await ExecuteDelegatesForGameObjects(preloadedEntities.Where(pe => pe is GameObject).Cast<GameObject>().ToList());
    }

    private async Task ExecuteDelegatesForScriptableObjects(List<ScriptableObject> preloadEntities)
    {
        foreach(ScriptableObject scriptableObject in preloadEntities)
        {
            if (scriptableObject is IDelegate)
            {
                ((IDelegate)scriptableObject).InvokeCustomMethod();
            }
        }
    }

    private async Task ExecuteDelegatesForGameObjects(List<GameObject> preloadedEntities)
    {
        foreach (GameObject preloadEntity in preloadedEntities)
        {
            List<IDelegate> iDelegates = new List<IDelegate>();

            iDelegates.AddRange(preloadEntity.gameObject.GetComponentsInChildren<IDelegate>());

            iDelegates.AddRange(preloadEntity.gameObject.GetComponents<IDelegate>());

            foreach (IDelegate del in iDelegates)
            {
                del.InvokeCustomMethod();
            }
        }
    }

    private async void PreloadEntitiesEventListener(List<UnityEngine.Object> preloadedEntities)
    {
        await ExecuteDelegates(preloadedEntities);
    }
    
}