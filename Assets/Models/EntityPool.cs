using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class EntityPool
{
    public string Name { get; set;}
    public string Tag { get; set; }
    public GameObject Entity { get ; set ; }
    public static Task<EntityPool> From(string name, string tag, GameObject entity)
    {
        return Task.FromResult(new EntityPool { Name = name, Tag = tag, Entity = entity });
    }
}