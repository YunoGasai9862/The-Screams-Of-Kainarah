using Annotations.Enums;
using Assets.Scripts.Interfaces.Registry;
using Assets.Scripts.Polling.Interfaces;
using Assets.Scripts.BaseScene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Asset(Asset.MONOBEHAVIOR, "SceneRegistry", InstantiationOrder = 1)]
public class SceneRegistry : MonoBehaviorScene, IRegistry, IPoller
{
    private Dictionary<Int32, GameObject> RegisteredGameObjects { get; set; } = new Dictionary<Int32, GameObject>();

    private Dictionary<Int32, ScriptableObject> RegisteredScriptObjects { get; set; } = new Dictionary<Int32, ScriptableObject>();

    private EntityPoolManager EntityPoolManagerInstance { get; set; }
    void Start()
    {
        FindObjectsByType<GameObject>(FindObjectsSortMode.None).Select(o => o.transform.root).ToList().ForEach(go => RegisteredGameObjects.TryAdd(go.GetInstanceID(), go.gameObject));

        EntityPoolManagerInstance = GetEntityPoolManager(RegisteredGameObjects);

        if (EntityPoolManagerInstance == null)
        {
            throw new ApplicationException($"EntityPoolManager is null...");
        }

        EntityPoolManagerInstance.GetPooledEntitiesWithAssetType(Asset.SCRIPTABLE_OBJECT).ForEach(so => RegisteredScriptObjects.Add(so.Entity.GetInstanceID(), so.Entity as ScriptableObject));
    }

    public Dictionary<Int32, GameObject> GetRegisteredGameObjects()
    {
        return RegisteredGameObjects;
    }

    public Dictionary<Int32, ScriptableObject> GetRegisteredScriptObjects()
    {
        return RegisteredScriptObjects;
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
        FindObjectsByType<GameObject>(FindObjectsSortMode.None).ToList().ForEach(go => RegisteredGameObjects.TryAdd(go.GetInstanceID(), go));

        EntityPoolManagerInstance.GetPooledEntitiesWithAssetType(Asset.SCRIPTABLE_OBJECT).ForEach(so => RegisteredScriptObjects.TryAdd(so.Entity.GetInstanceID(), so.Entity as ScriptableObject));

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

    public UnityEngine.Object GetRegisteredObject(Asset assetType, string objectName)
    {
        switch(assetType)
        {
            case Asset.MONOBEHAVIOR:
                if (RegisteredGameObjects.Count == 0)
                {
                    throw new ApplicationException($"No game objects found in the registry: {nameof(SceneRegistry)}");
                }

                List<GameObject> gameObjects = RegisteredGameObjects.Where(go => go.Value.name.Equals(objectName, StringComparison.OrdinalIgnoreCase)).Select(kvp => kvp.Value).ToList();

                if (gameObjects.Count > 0)
                {
                    throw new ApplicationException($"Multiple game object instances found with the name {objectName} in the registry: {nameof(SceneRegistry)}");
                }

                return gameObjects.FirstOrDefault();
            case Asset.SCRIPTABLE_OBJECT:
                if (RegisteredScriptObjects.Count == 0)
                {
                    throw new ApplicationException($"No scriptable objects found in the registry: {nameof(SceneRegistry)}");
                }

                List<ScriptableObject> scriptableObjects = RegisteredScriptObjects.Where(go => go.Value.name.Equals(objectName, StringComparison.OrdinalIgnoreCase)).Select(kvp => kvp.Value).ToList();

                if (scriptableObjects.Count > 0)
                {
                    throw new ApplicationException($"Multiple scriptableObjects found with the name {objectName} in the registry: {nameof(SceneRegistry)}");
                }

                return scriptableObjects.FirstOrDefault();
            default:
                throw new ApplicationException($"Type {assetType} is not supported for retrieval in {nameof(SceneRegistry)}");
        }

 
    }
}
