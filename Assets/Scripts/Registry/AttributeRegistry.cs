
using Assets.Annotations;
using Assets.Scripts.Interfaces.Registry;
using Assets.Scripts.ObserverPattern.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.Registry
{
    public class AttributeRegistry : MonoBehaviour, IAttributeRegistry
    {
        private List<Type> Assemblies { get; set; } = new List<Type>();

        private void Start()
        {
            Assemblies = CacheAssemblies();
        }

        public List<T> GetAttributes<T>(List<Type> nonGenericTypes = null, List<Type> genericTypes = null) where T : Attribute
        {
            try
            {

                return Helper.GetAttribute<T>(Assemblies, genericTypes, nonGenericTypes).ToList();

                //List<ObserverAttribute> observers = Helper.GetAttribute<ObserverAttribute>(
                //         executingAssemblyTypes, new List<Type>
                //         {
                //         typeof(Assets.Scripts.Interfaces.Mediator.Base.INotify<>),
                //         typeof(Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<>),
                //         },
                //         new List<Type>
                //         {
                //         typeof(Assets.Scripts.Interfaces.Mediator.Base.INotify),
                //         typeof(Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify)
                //         }
                //    ).ToList();

                //Debug.Log($"Observers found: {observers.Count}");

                //foreach (SubjectAttribute subject in subjects)
                //{
                //    if (subject.AssetType == Asset.NONE || subject.EntityType == null || subject.ContextType == null)
                //    {
                //        Debug.LogWarning($"Either AssetType, SubjectType or ContextType is missing for the subject attribute: {subject}. Skipping the registration for this subject!");
                //        continue;
                //    }

                //    SubjectBundle subjectBundle = new SubjectBundle() { SubjectAttribute = subject };

                //    List<ObserverAttribute> specificObservers = observers.Where(observer => observer.SubjectType.Equals(subject.EntityType)).ToList();

                //    foreach (ObserverAttribute observer in specificObservers)
                //    {
                //        if (observer.AssetType == Asset.NONE || observer.SubjectType == null || observer.ContextType == null || observer.EntityType == null)
                //        {
                //            Debug.LogWarning($"Either AssetType, SubjectType, ContextType, or Observertype is missing for the observer attribute: {observer}. Skipping the registration for this observer!");
                //            continue;
                //        }

                //        ObserverBundle observerBundle = new ObserverBundle()
                //        {
                //            ObserverAttribute = observer
                //        };

                //        Debug.Log("SubjectBundle: " + subjectBundle.ToString() + " " + "Observerbundle: " + observerBundle.ToString());

                //        //check if exists - append, otherwise create a new list!!!
                //        if (!Associations.TryGetValue(subjectBundle, out List<dynamic> observerBundles))
                //        {
                //            Debug.Log($"Adding to association: {subjectBundle?.SubjectAttribute?.EntityType} - {observerBundle?.ObserverAttribute?.EntityType}");
                //            Associations[subjectBundle] = new List<dynamic>() { observerBundle };
                //            continue;
                //        }

                //        Debug.Log($"Adding to association: {subjectBundle?.SubjectAttribute?.EntityType} - {observerBundle?.ObserverAttribute?.EntityType}");

                //        Associations[subjectBundle].Add(observerBundle);
                //    }
                //}

                //Debug.Log("Done Registering...");

                //RegistryState = Registry.REGISTRY_READY;
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

        public bool DecomissionAttribute<T>(T attribute)
        {
            throw new NotImplementedException();
        }
    }
}
