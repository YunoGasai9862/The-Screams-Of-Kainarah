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

    private GameLoad  { get; set; }
    void Start()
    {
        FindObjectsByType<GameObject>(FindObjectsSortMode.None).ToList().ForEach(go => RegisteredGameObjects.Add(go.GetInstanceID(), go));

        KeyValuePair<Int32, GameObject> GameLoadKVP = RegisteredGameObjects.First(kvp => kvp.Value.TryGetComponent<GameLoad>(out GameLoad GameLoadInstance));
    }

    void Update() 
    {
        
    }
    public void Decommission(Int32 instanceId)
    {
        throw new System.NotImplementedException();
    }

    public void Register<T>(T value, Asset assetType)
    {
       
    }
}
