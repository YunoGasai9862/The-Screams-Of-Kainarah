
using Annotations.Enums;
using System;

namespace Assets.Scripts.Interfaces.Registry
{
    public interface IRegistry
    {
        bool Decommission(Int32 instanceId, Asset assetType);

        bool Register<T>(T value, Asset assetType) where T : UnityEngine.Object;
    }
}
