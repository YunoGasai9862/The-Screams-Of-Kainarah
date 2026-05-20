using Annotations.Enums;
using Assets.Scripts.Interfaces.Registry;
using Assets.Scripts.Polling.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Asset(Asset.MONOBEHAVIOR, "SceneRegistry", InstantiationOrder = 1)]
public class SceneRegistry : MonoBehaviour, IRegistry, IPoller
{
    private Dictionary<Int32, GameObject> RegisteredGameObjects { get; set; } = new Dictionary<Int32, GameObject>();

    private Dictionary<Int32, ScriptableObject> RegisteredScriptObjects { get; set; } = new Dictionary<Int32, ScriptableObject>();

    private EntityPoolManager EntityPoolManagerInstance { get; set; }
    void Start()
    {
        FindObjectsByType<GameObject>(FindObjectsSortMode.None).ToList().ForEach(go => RegisteredGameObjects.Add(go.GetInstanceID(), go));

        EntityPoolManagerInstance = GetEntityPoolManager(RegisteredGameObjects);

        if (EntityPoolManagerInstance == null)
        {
            throw new ApplicationException($"EntityPoolManager is null...");
        }
    }

    public bool Decommission(Int32 instanceId, Asset assetType)
    { 
        switch (assetType)
        {
            case Asset.MONOBEHAVIOR: return RegisteredGameObjects.Remove(instanceId);
            case Asset.SCRIPTABLE_OBJECT: return RegisteredScriptObjects.Remove(instanceId);
            case Asset.NONE: break;
            case Asset.PLAYER_STATE_MACHINE: break;
        }

        throw new ApplicationException($"Asset type {assetType} is not supported for registration in {nameof(SceneRegistry)}");
    }

    public bool Register<T>(T value, Asset assetType) where T: UnityEngine.Object
    {
       switch(assetType)
        {
            case Asset.MONOBEHAVIOR:
                return RegisteredGameObjects.TryAdd(value.GetInstanceID(), value as GameObject);
            case Asset.SCRIPTABLE_OBJECT:
                return RegisteredScriptObjects.TryAdd(value.GetInstanceID(), value as ScriptableObject);
            case Asset.NONE: break;
            case Asset.PLAYER_STATE_MACHINE: break;
        }
        
        throw new ApplicationException($"Asset type {assetType} is not supported for registration in {nameof(SceneRegistry)}");
    }

    private EntityPoolManager GetEntityPoolManager(Dictionary<Int32, GameObject> registeredGameObjects)
    {
        EntityPoolManager entityPoolManager = null;

        foreach (KeyValuePair<Int32, GameObject> item in registeredGameObjects)
        {
            if (item.Value.TryGetComponent(out EntityPoolManager entityPoolManagerInstance))
            {
                entityPoolManager = entityPoolManagerInstance;
            }
        }

        return entityPoolManager;
    }

    public IEnumerator ScanScene(int pollingIntervalInSeconds)
    {
        FindObjectsByType<GameObject>(FindObjectsSortMode.None).ToList().ForEach(go => RegisteredGameObjects.Add(go.GetInstanceID(), go));

        yield return new WaitForSeconds(pollingIntervalInSeconds);
    }

    private void OnDisable()
    {
        RegisteredGameObjects.Clear();
        RegisteredScriptObjects.Clear();

        StopAllCoroutines();
    }

    public IEnumerator Poll(int pollingIntervalInSeconds)
    {
        yield return StartCoroutine(ScanScene(pollingIntervalInSeconds));
    }
}
