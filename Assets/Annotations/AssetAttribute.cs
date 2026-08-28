using Annotations.Enums;
using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AssetAttribute: Attribute
{
    public Asset AssetType { get; set; }

    public string AddressLabel { get; set; }

    public int InstantiationOrder { get; set; }

    public string[] MarkerIds { get; set; }

    public AssetAttribute(Asset assetType, string addressLabel)
    {
        AssetType = assetType;
        AddressLabel = addressLabel;
    }

    public AssetAttribute(Asset assetType, string addressLabel, int instantiationOrder, string[] markerId) { 
    
       AssetType = assetType;
       AddressLabel = addressLabel;
       InstantiationOrder = instantiationOrder;
       MarkerIds = markerId;
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