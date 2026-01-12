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
    //use subject bundle for storing subject's instance???
    private Dictionary<SubjectBundle, ObserverBundle> Associations { get; set; } = new Dictionary<SubjectBundle, ObserverBundle>();

    private List<Type> ExecutingAssemblyTypes { get; set; } = new List<Type>();

    private void Awake()
    {
        BuildRegistry();
    }

    //TODO
    //need to implement retry mechanism - for the Subject instance
    public IEnumerator NotifyObserver<T>(SubjectContext<T> context, CancellationToken cancellationToken, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        KeyValuePair<SubjectBundle, ObserverBundle> association = Associations.Where(kvp => kvp.Key.SubjectAttribute.SubjectType == context.EntityType).FirstOrDefault();

        if (association.Value == null)
        {
            throw new MissingContractException($"No observer found for the subject type: {association.Key.SubjectAttribute.SubjectType}!");
        }

        List<ObserverContext> cachedObserverContext = GetObserverContext<T, SubjectContext<T>> (association, context);

        if (cachedObserverContext == null || cachedObserverContext.Count == 0)
        {
            Debug.LogWarning($"The cached observers are null or either have not broadcasted their presence. Retrying...");

            StartCoroutine(NotifyObserver<T>(context, cancellationToken, maxRetries - 1, sleepTimeInMilliSeconds, semaphoreSlim, optional));
        }

        cachedObserverContext.ForEach(observer =>
        {
            INotify<T> observerNotify = observer.Instance.GetComponent<INotify<T>>();

            if (observerNotify == null)
            {
                throw new MissingContractException($"The observer instance does not implement the INotify<{typeof(T).Name}");
            }

            observerNotify.Notify(context.Data);
        });

        yield return null;
    }

    //TODO
    //need to implement retry mechanism - for the Observer Instance
    public IEnumerator NotifySubject<T>(ObserverContext<T> context, CancellationToken cancellationToken, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        if (maxRetries == 0)
        {
            throw new MissingContextException($"Unable to fish for the subject type within the scene: {context.SubjectType}!");
        }

        if (context == null || context.SubjectType == null || context.Instance == null)
        {
            throw new MissingContextException($"Either the context is null or SubjectType/Instance are missing from the instance!");
        }

        //once the dictionary has been built, we need to query live instances to make sure what observer is claiming, really exists!!
        KeyValuePair<SubjectBundle, ObserverBundle> association = Associations.Where(kvp => kvp.Key.SubjectAttribute.SubjectType == context.SubjectType).FirstOrDefault();

        if (association.Value == null)
        {
            throw new MissingContractException($"No observer found for the subject type: {context.SubjectType}!");
        }

        //now start building out the logic
        ObserverContext cachedObserverContext = GetObserverContext<T, ObserverContext<T>>(association.Value.ObserverContexts, context);

        if (cachedObserverContext == null)
        {
            Associations[association.Key].ObserverContexts.Add(context);
        }

        //will need to retry if instance is null
        if (association.Key.SubjectContext.Instance == null)
        {
            Debug.LogWarning($"The subject instance is null for the subject type: {context.SubjectType}. Attemping a retry...");

            StartCoroutine(NotifySubject<T>(context, cancellationToken, maxRetries - 1, sleepTimeInMilliSeconds, semaphoreSlim, optional));
        }

        //see if its better to store it??
        IRequest<T> subjectRequest = association.Key.SubjectContext.Instance.GetComponent<IRequest<T>>();

        if (subjectRequest == null)
        {
            throw new MissingContractException($"The subject instance does not implement the IRequest<{typeof(T).Name}");
        }

        subjectRequest.Request();

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

                Associations.Add(new SubjectBundle() {SubjectAttribute = subject }, bundle);
            });

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

    private ObserverContext GetObserverContext<T, Z>(List<ObserverContext> observerContexts, Z context) where Z: ObserverContext<T>
    {
        return observerContexts.Where(observerContext => observerContext.Instance.name.Equals(context.Instance.name) &&
                                                         observerContext.Instance.tag.Equals(context.Instance.tag) &&
                                                         observerContext.SubjectType.Equals(context.SubjectType)).FirstOrDefault();
    }

    private List<ObserverContext> GetObserverContext<T, Z>(KeyValuePair<SubjectBundle, ObserverBundle> association, Z context) where Z : SubjectContext<T>
    {
        return association.Value.ObserverContexts.Where(observerContext => observerContext.Instance.name.Equals(context.Instance.name) &&
                                                                           observerContext.Instance.tag.Equals(context.Instance.tag) && 
                                                                           observerContext.SubjectType.Equals(context.EntityType)).ToList();
    }

}