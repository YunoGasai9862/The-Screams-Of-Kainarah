using System;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class EntityPool
{
    public UnityEngine.Object Entity { get ; set ; } 
    public string Name { get; set; }
    public string Tag { get; set; }
    public static Task<EntityPool> From(string name, string tag, UnityEngine.Object entity)
    {
        EntityPool entityPool = new EntityPool { Name = name, Tag = tag, Entity = entity };

        return Task.FromResult(entityPool);
    }

    public override string ToString()
    {
        return $"Name: {Name}, Tag: {Tag}, Entity : {Entity}";
    }
}