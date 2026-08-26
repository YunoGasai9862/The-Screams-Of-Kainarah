using Annotations.Enums;
using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AssetAttribute: Attribute
{
    public Asset AssetType { get; set; }

    public string AddressLabel { get; set; }

    public int InstantiationOrder { get; set; }

    public int NumberOfInstancesToInitiate { get; set; }

    public float InitialPositionX { get; set; } = default;

    public float InitialPositionY { get; set; } = default;

    public float InitialPositionZ { get; set; } = default;

    public AssetAttribute(Asset assetType, string addressLabel)
    {
        AssetType = assetType;
        AddressLabel = addressLabel;
    }

    public AssetAttribute(Asset assetType, string addressLabel, int instantiationOrder, int numberOfInstancesToInitiate) { 
    
       AssetType = assetType;
       AddressLabel = addressLabel;
       InstantiationOrder = instantiationOrder;
       NumberOfInstancesToInitiate = numberOfInstancesToInitiate;
    }

    public AssetAttribute(Asset assetType, string addressLabel, int instantiationOrder, int numberOfInstancesToInitiate, float initialPositionX, float initialPositionY, float initialPositionZ)
    {
        AssetType = assetType;
        AddressLabel = addressLabel;
        InstantiationOrder = instantiationOrder;
        NumberOfInstancesToInitiate = numberOfInstancesToInitiate;
        InitialPositionX = initialPositionX;
        InitialPositionY = initialPositionY;
        InitialPositionZ = initialPositionZ;    
    }


    public override string ToString()
    {
        return $"AssetAttribute: Type: {AssetType}, Label: {AddressLabel} InstantiationOrder: {InstantiationOrder}, NumberOfInstancesToInitiate: {NumberOfInstancesToInitiate}";
    }
}