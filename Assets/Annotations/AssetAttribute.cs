using System;
using System.Numerics;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AssetAttribute: Attribute
{
    public Asset AssetType { get; set; }

    public string AddressLabel { get; set; }

    public float PositionX { get; set; }

    public float PositionY { get; set; } 

    public float PositionZ { get; set; }

    public AssetAttribute()
    {

    }

    public AssetAttribute(Asset assetType, string addressLabel) { 
    
       AssetType = assetType;
       AddressLabel = addressLabel;
    }

    public override string ToString()
    {
        return $"AssetAttribute: Type: {AssetType}, Label: {AddressLabel}";
    }
}