using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class EntityPool<T> : AbstractEntityPool
{
    public new T Entity { get ; set ; }
    public static Task<EntityPool<T>> From(string name, string tag, T entity)
    {
        EntityPool<T> entityPool = new EntityPool<T> { Name = name, Tag = tag, Entity = entity };

        return Task.FromResult(entityPool);
    }
    
    public override string ToString()
    {
        return $"Name: {Name}, Tag: {Tag}, GameObject: {Entity}";
    }
}