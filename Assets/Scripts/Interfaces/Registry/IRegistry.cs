
using Annotations.Enums;
using System;

namespace Assets.Scripts.Interfaces.Registry
{
    public interface IRegistry
    {
        void Decommission(Int32 instanceId);

        void Register<T>(T value, Asset assetType);
    }
}
