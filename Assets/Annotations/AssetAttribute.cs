using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AssetAttribute: Attribute
{
    public Asset AssetType { get; set; }

    public string AddressLabel { get; set; }
    public AssetAttribute()
    {

    }
    public AssetAttribute(Asset assetType, string addressLabel) { 
    
       AssetType = assetType;
       AddressLabel = addressLabel;
    }
}