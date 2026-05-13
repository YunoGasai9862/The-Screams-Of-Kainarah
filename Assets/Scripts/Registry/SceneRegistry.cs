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

    private GameLoad GameLoad { get; set; }
    void Start()
    {
        FindObjectsByType<GameObject>(FindObjectsSortMode.None).ToList().ForEach(go => RegisteredGameObjects.Add(go.GetInstanceID(), go));

        GameLoad = GetGameLoad(RegisteredGameObjects);

        if (GameLoad == null)
        {
            throw new ApplicationException($"GameLoad is null...");
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

    private GameLoad GetGameLoad(Dictionary<Int32, GameObject> registeredGameObjects)
    {
        GameLoad gameLoadInstance = null;

        foreach (KeyValuePair<Int32, GameObject> item in registeredGameObjects)
        {
            if (item.Value.TryGetComponent(out GameLoad gameload))
            {
                gameLoadInstance = gameload;
            }
        }

        return gameLoadInstance;
    }

    public IEnumerator ScanScene(int scanIntervalInSeconds = 60)
    {
        FindObjectsByType<GameObject>(FindObjectsSortMode.None).ToList().ForEach(go => RegisteredGameObjects.Add(go.GetInstanceID(), go));

        yield return null;
    }

    private void OnDisable()
    {
        RegisteredGameObjects.Clear();
        RegisteredScriptObjects.Clear();

        StopAllCoroutines();
    }

    public IEnumerator Poll()
    {
        yield return StartCoroutine(ScanScene());
    }
}
