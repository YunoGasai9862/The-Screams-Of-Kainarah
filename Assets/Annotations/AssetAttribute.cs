using System;
using System.Numerics;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AssetAttribute: Attribute
{
    public Asset AssetType { get; set; }

    public string AddressLabel { get; set; }

    public float InitialPositionX { get; set; } = default;

    public float InitialPositionY { get; set; } = default;

    public float InitialPositionZ { get; set; } = default;

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