
using Annotations.Enums;
using Assets.Scripts.Interfaces.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.Registry
{
    [Asset(Asset.MONOBEHAVIOR, "AttributeRegistry", InstantiationOrder = 2)]
    public class AttributeRegistry : Scene, IAttributeRegistry
    {
        private List<Type> Assemblies { get; set; } = new List<Type>();

        private Dictionary<Type, List<Attribute>> Attributes { get; set; } = new Dictionary<Type, List<Attribute>>();

        private void Start()
        {
            Assemblies = CacheAssemblies();

            Attributes = BuildAttributeRegistry(Assemblies);
        }

        public List<T> GetAttributes<T>(List<Type> genericTypes = null, List<Type> nonGenericTypes = null) where T : Attribute
        {
            try
            {
                return SceneUtils.GetAttribute<T>(Assemblies, genericTypes, nonGenericTypes).ToList();
            }
            catch (BaseException ex)
            {
                Debug.Log($"Exception: {ex.Message}");
            }

            return new List<T>();
        }

        public List<Type> CacheAssemblies()
        {
            Debug.Log($"Caching Assemblies...");

            return Assembly.GetExecutingAssembly().GetTypes().ToArray().ToList();
        }

        public Dictionary<Type, List<Attribute>> BuildAttributeRegistry(List<Type> assemblies)
        {
            Dictionary<Type, List<Attribute>> attributesRegistry = new Dictionary<Type, List<Attribute>>();

            assemblies.ForEach(assembly =>
            {
                List<Attribute> attributes = assembly.GetCustomAttributes().ToList();

                attributes.ForEach(attribute =>
                {
                    if (attributesRegistry.TryGetValue(attribute.GetType(), out List<Attribute> existingAttributes))
                    {
                        existingAttributes.Add(attribute);
                    }else
                    {
                        attributesRegistry[attribute.GetType()] = new List<Attribute>() { attribute };
                    }
                });
            });

            return attributesRegistry;
        }

        public void DecommissionAssemblies()
        {
            Assemblies.Clear();
        }
    }
}
