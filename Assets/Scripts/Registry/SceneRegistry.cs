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
    void Start()
    {
        FindObjectsByType<GameObject>(FindObjectsSortMode.None).ToList().ForEach(go => RegisteredGameObjects.Add(go.GetInstanceID(), go));
    }

    void Update()
    {
        
    }
    public void Decommission(Int32 instanceId)
    {
        throw new System.NotImplementedException();
    }

    public void Register<T>(T value)
    {
        throw new System.NotImplementedException();
    }
}
