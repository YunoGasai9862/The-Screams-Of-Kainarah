using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Assets.Scripts.Interfaces.Registry
{
    public interface IAttributeRegistry
    {
        List<Type> CacheAssemblies();

        void DecommissionAssemblies();

        Dictionary<Type, List<Attribute>> BuildAttributeRegistry(List<Type> assemblies);

        Task<List<T>> GetAttributes<T>(List<Type> nonGenericTypes = null, List<Type> genericTypes = null) where T : Attribute;
    }
}
