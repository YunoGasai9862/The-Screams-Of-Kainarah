using Annotations.Enums;
using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AssetAttribute: Attribute
{
    public Asset AssetType { get; set; }

    public string AddressLabel { get; set; }

    public int InstantiationOrder { get; set; }

    public string[] MarkerIds { get; set; } = new string[0];

    public bool ExternalDependency { get; set; }

    public AssetAttribute(Asset assetType, string addressLabel)
    {
        AssetType = assetType;
        AddressLabel = addressLabel;
    }

    public AssetAttribute(Asset assetType, string addressLabel, string[] markerId, bool externalDependency)
    {
        AssetType = assetType;
        AddressLabel = addressLabel;
        MarkerIds = markerId;
        ExternalDependency = externalDependency;
    }

    public AssetAttribute(Asset assetType, string addressLabel, int instantiationOrder, string[] markerId, bool externalDependency) { 
    
       AssetType = assetType;
       AddressLabel = addressLabel;
       InstantiationOrder = instantiationOrder;
       MarkerIds = markerId;
       ExternalDependency = externalDependency;
    }

    public AssetAttribute(Asset assetType, string addressLabel, int instantiationOrder)
    {
        AssetType = assetType;
        AddressLabel = addressLabel;
        InstantiationOrder = instantiationOrder; 
    }


    public override string ToString()
    {
        return $"AssetAttribute: Type: {AssetType}, Label: {AddressLabel} InstantiationOrder: {InstantiationOrder}, MarkerId: {string.Join(",", MarkerIds)}";
    }
}