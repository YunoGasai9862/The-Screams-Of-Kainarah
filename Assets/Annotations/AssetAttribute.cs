using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AssetAttribute: Attribute
{
    public Asset AssetType { get; set; }
    public AssetAttribute()
    {

    }
    public AssetAttribute(Asset assetType) { 
    
       AssetType = assetType;
    }
}