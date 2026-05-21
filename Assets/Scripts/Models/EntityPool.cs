using Annotations.Enums;
using System;
using System.Threading.Tasks;

[Serializable]
public class EntityPool
{
    public UnityEngine.Object Entity { get ; set ; } 
    public string Name { get; set; }
    public string Tag { get; set; }

    public Asset AssetType { get; set; }
    public static Task<EntityPool> From(string name, string tag, Asset assetType, UnityEngine.Object entity)
    {
        EntityPool entityPool = new EntityPool { Name = name, Tag = tag, AssetType = assetType, Entity = entity };

        return Task.FromResult(entityPool);
    }

    public override string ToString()
    {
        return $"Name: {Name}, Tag: {Tag}, AssetType: {AssetType},  Entity : {Entity}";
    }
}