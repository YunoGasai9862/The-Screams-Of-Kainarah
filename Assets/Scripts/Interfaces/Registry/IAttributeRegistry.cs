using System;
using System.Collections.Generic;
namespace Assets.Scripts.Interfaces.Registry
{
    public interface IAttributeRegistry
    {
        List<Type> CacheAssemblies();

        bool DecomissionAttribute<T>(T attribute);

        List<T> GetAttributes<T>(List<Type> nonGenericTypes = null, List<Type> genericTypes = null) where T : Attribute;
    }
}
