using Assets.Annotations;
using Assets.Exceptions;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using Assets.Scripts.ObserverPattern.interfaces;
using Assets.Scripts.ObserverPattern.models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Delegator : MonoBehaviour, IDelegator
{
    private Dictionary<ISubjectBundle, List<IObserverBundle>> Associations { get; set; } = new Dictionary<ISubjectBundle, List<IObserverBundle>>();

    private List<Type> ExecutingAssemblyTypes { get; set; } = new List<Type>();

    private void Awake()
    {
        BuildRegistry();
    }

    public void NotifyObserverWrapper<T>(SubjectContext<T> context, IRequest<T> subject, INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifyObserver(context, subject, observer, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public void NotifyObserverWrapper<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV2.IRequest<T> subject, INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifyObserver(context, subject, observer, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public IEnumerator NotifyObserver<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV2.IRequest<T> subject, INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        yield return null;
    }

    public IEnumerator NotifyObserver<T>(SubjectContext<T> context, IRequest<T> subject, INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        yield return null;
    }

    public IEnumerator NotifyObservers<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV2.IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        yield return StartCoroutine(NotifyObservers<T>(context, (IRequest<T>) subject, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public IEnumerator NotifyObservers<T>(SubjectContext<T> context, IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        KeyValuePair<ISubjectBundle, List<IObserverBundle>> association = Associations.Where(kvp => kvp.Key.SubjectAttribute.SubjectType == context.EntityType).FirstOrDefault();

        if (association.Value == null)
        {
            throw new MissingContractException($"No observer found for the subject type: {association.Key.SubjectAttribute.SubjectType}!");
        }


        if (association.Key.Subject == null)
        {
            Debug.LogWarning($"The subject instance is null for the subject type: {context.EntityType}. Will update the dictionary with the current instance!");

            //check later if the casting will work seamlessly
            association.Key.Subject = subject;
        }

        List<INotify> cachedObserverContext = GetObserverBundles<T, SubjectContext<T>> (association, context);

        if (cachedObserverContext == null || cachedObserverContext.Count == 0)
        {
            Debug.LogWarning($"The cached observers are null or either have not broadcasted their presence. Retrying...");

            yield return new WaitForSeconds(sleepTimeInMilliSeconds);

            yield return StartCoroutine(NotifyObservers<T>(context, subject, maxRetries - 1, sleepTimeInMilliSeconds, optional));
        }

        cachedObserverContext.ForEach(observer =>
        {
            //check later if the casting will work seamlessly
            INotify<T> observerNotify = (INotify<T>) observer;

            if (observerNotify == null)
            {
                throw new MissingContractException($"The observer instance does not implement the INotify<{typeof(T).Name}");
            }

            observerNotify.Notify(context.Data);
        });

        yield return null;
    }

    public IEnumerator NotifySubject<T>(ObserverContext<T> context, INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        if (maxRetries == 0)
        {
            throw new MissingContextException($"Unable to fish for the subject type within the scene: {context.SubjectType}!");
        }

        if (context == null || context.SubjectType == null || context.Instance == null)
        {
            throw new MissingContextException($"Either the context is null or SubjectType/Instance are missing from the instance!");
        }

        KeyValuePair<ISubjectBundle, List<IObserverBundle>> association = Associations.Where(kvp => kvp.Key.SubjectAttribute.SubjectType == context.SubjectType).FirstOrDefault();

        if (association.Value == null || association.Value.Count == 0)
        {
            throw new MissingContractException($"No observer found for the subject type: {context.SubjectType}!");
        }

        IObserverBundle cachedObserverContext = GetObserverBundle<T, ObserverContext>(association.Value, context);

        if (cachedObserverContext.Observer == null)
        {
            cachedObserverContext.Observer = (INotify) observer;
            Associations[association.Key].Add(cachedObserverContext);
        }

        if (association.Key.Subject == null)
        {
            Debug.LogWarning($"The subject instance is null for the subject type: {context.SubjectType}. Attemping a retry...");

            yield return new WaitForSeconds(sleepTimeInMilliSeconds);

            yield return StartCoroutine(NotifySubject<T>(context, observer, maxRetries - 1, sleepTimeInMilliSeconds, optional));
        }

        //see if its better to store it?? (compare letter the difference/performance)
        IRequest<T> subjectRequest = (IRequest<T>) association.Key.Subject;

        if (subjectRequest == null)
        {
            throw new MissingContractException($"The subject instance does not implement the IRequest<{typeof(T).Name}");
        }

        subjectRequest.Request();

        yield return null;
    }


    public void NotifyObserversWrapper<T>(SubjectContext<T> context, IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifyObservers(context, subject, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public void NotifySubjectWrapper<T>(ObserverContext<T> context, INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifySubject(context, observer, maxRetries, sleepTimeInMilliSeconds, optional));
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

            foreach (SubjectAttribute subject in subjects)
            {
                ObserverBundle observerBundle = new ObserverBundle()
                {
                    ObserverAttribute = observers.Find(observer => observer.SubjectType.Equals(subject.SubjectType))
                };

                SubjectBundle subjectBundle = new SubjectBundle() { SubjectAttribute = subject };

                //check if exists - append, otherwise create a new list!!!
                if (Associations[subjectBundle] == null)
                {
                    Associations[subjectBundle] = new List<IObserverBundle>() { observerBundle };
                    continue;
                }

                Associations[subjectBundle].Add(observerBundle);
            }

        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    private HashSet<T> Find<T>(List<Type> types, Type requiredInterfaceType = null) where T : Attribute
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

    private IObserverBundle GetObserverBundle<T, Z>(List<IObserverBundle> observers, Z context) where Z: ObserverContext
    {
        return observers.Where(observerContext => observerContext.ObserverAttribute.ObserverType.Equals(context.EntityType) &&
                                                    typeof(T).Name.Equals(observerContext.ObserverAttribute.ContextType) && 
                                                    observerContext.ObserverAttribute.SubjectType.Equals(context.SubjectType)).FirstOrDefault();
    }

    private List<INotify> GetObserverBundles<T, Z>(KeyValuePair<ISubjectBundle, List<IObserverBundle>> association, Z context) where Z : SubjectContext<T>
    {
        return association.Value.Where(observerContext => observerContext.ObserverAttribute.SubjectType.Equals(context.EntityType) && typeof(T).Name.Equals(context.Data.GetType())).Select(observer => observer.Observer).ToList();
    }
}