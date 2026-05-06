using System;
using System.Collections.Generic;
namespace Assets.Scripts.Interfaces.Registry
{
    public interface IAttributeRegistry
    {
        void CacheAssemblies();

        bool DecomissionAttribute<T>(T attribute);

        bool GetAttributes<T>(List<Type> filteringTypes, List<Type> nonGenericInterfaceFilteringTypes = null, List<Type> genericInterfaceFilteringType = null) where T : Attribute;
    }
}
