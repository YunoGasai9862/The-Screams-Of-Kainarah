using System;
namespace Assets.Scripts.Interfaces.Registry
{
    public interface IAttributeRegistry
    {
        bool DecomissionAttribute<T>(T attribute);

        bool BuildAttributeRegistry<T>(T value) where T : Attribute;
    }
}
