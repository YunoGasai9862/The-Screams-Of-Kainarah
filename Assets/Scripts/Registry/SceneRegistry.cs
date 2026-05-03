using Annotations.Enums;
using Assets.Scripts.Interfaces.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Asset(Asset.MONOBEHAVIOR, "SceneRegistry", InstantiationOrder = 1)]
public class SceneRegistry : MonoBehaviour, IRegistry
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

    public void Decommission(Int32 instanceId)
    {
        
    }

    public bool Register<T>(T value, Asset assetType) where T: UnityEngine.Object
    {
       switch(assetType)
        {
            case Asset.MONOBEHAVIOR:
                return RegisteredGameObjects.TryAdd(value.GetInstanceID(), value as GameObject);
            case Asset.SCRIPTABLE_OBJECT: break;
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
}
