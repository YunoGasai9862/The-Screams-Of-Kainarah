using Assets.Annotations;
using Assets.Exceptions;
using Assets.Scripts.Interfaces;
using Assets.Scripts.ObserverPattern.models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class Delegator : MonoBehaviour, IDelegator
{
    private Dictionary<SubjectAttribute, ObserverBundle> Associations { get; set; } = new Dictionary<SubjectAttribute, ObserverBundle>();

    private List<Type> ExecutingAssemblyTypes { get; set; } = new List<Type>();

    private void Awake()
    {
        BuildRegistry();
    }

    public IEnumerator NotifyObserver<T>(SubjectContext<T> context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        KeyValuePair<SubjectAttribute, ObserverBundle> association = Associations.Where(kvp => kvp.Key.SubjectType == context.EntityType).FirstOrDefault();

        if (association.Value == null)
        {
            throw new MissingContractException($"No subject found for : {context.EntityType}!");
        }

        ObserverContext cachedObserverContext = association.Value.ObserverContexts.Where(observerContext => observerContext.Name.Equals(context.Name) && observerContext.Tag.Equals(context.Tag) && observerContext.SubjectType.Equals(context.EntityType)).FirstOrDefault();


        yield return null;
    }

    public IEnumerator NotifySubject<T>(ObserverContext<T> context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        if (context == null || context.Name == null || context.Tag == null || context.SubjectType == null || context.Instance == null)
        {
            throw new MissingContextException($"Either the context is null or name/tag/SubjectType/Instance are missing from the instance!");
        }

        //once the dictionary has been built, we need to query live instances to make sure what observer is claiming, really exists!!
         
        KeyValuePair<SubjectAttribute, ObserverBundle> association = Associations.Where(kvp => kvp.Key.SubjectType == context.SubjectType).FirstOrDefault();
        
        if (association.Value == null)
        {
            throw new MissingContractException($"No observer found for the subject type: {context.SubjectType}!");
        }

        //now start building out the logic
        List<ObserverContext> observerContexts = association.Value.ObserverContexts;

        ObserverContext cachedObserverContext = observerContexts.Where(observerContext => observerContext.Name.Equals(context.Name) && observerContext.Tag.Equals(context.Tag) && observerContext.SubjectType.Equals(context.SubjectType)).FirstOrDefault();

        if (cachedObserverContext == null)
        {
            Associations[association.Key].ObserverContexts.Add(context);
        }



        //keep building/storing the instances
        yield return null;
    }

    //we should check on generic interface assignment since we wouldn't know concrete implementation during reflection.
    //in order to do that, get interfaces first and then check on IsGenericFlag and TypeDefinition
    public void BuildRegistry()
    {
        try
        {
            ExecutingAssemblyTypes = Assembly.GetExecutingAssembly().GetTypes().ToArray().ToList();

            List<SubjectAttribute> subjects = Find<SubjectAttribute>(ExecutingAssemblyTypes, typeof(IRequest<>)).ToList();

            List<ObserverAttribute> observers = Find<ObserverAttribute>(ExecutingAssemblyTypes, typeof(INotify<>)).ToList();

            subjects.ForEach(subject =>
            {
                ObserverBundle bundle = new ObserverBundle()
                {

                    ObserverAttribute = observers.Find(observer => observer.SubjectType.Equals(subject.SubjectType))
                };

                Associations.Add(subject, bundle);
            });

        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    private HashSet<T> Find<T>(List<Type> types, Type requiredInterfaceType = null) where T: Attribute
    {
        HashSet<T> foundAttributes = new HashSet<T>();

        foreach (Type type in types)
        {
            T attribute = type.GetCustomAttribute<T>();

            if (attribute == null)
            {
                Debug.Log($"No {attribute} found for type: {type.FullName}");

                continue;
            }

            if (!type.GetInterfaces().Any(interf => requiredInterfaceType != null && interf.IsGenericType && interf.GetGenericTypeDefinition() == requiredInterfaceType))
            {
                throw new MissingContractException($"The underlying type must implement {requiredInterfaceType}!");
            }
        }

        return foundAttributes;
    }
}