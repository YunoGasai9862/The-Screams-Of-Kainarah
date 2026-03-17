using Assets.Annotations;
using Assets.Exceptions;
using Assets.Scripts.ObserverPattern.interfaces;
using Assets.Scripts.ObserverPattern.models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Assets.Scripts.Enums;

public class Delegator : MonoBehaviour, IDelegator
{
    private Dictionary<ISubjectBundle, List<IObserverBundle>> Associations { get; set; } = new Dictionary<ISubjectBundle, List<IObserverBundle>>();

    private List<Type> ExecutingAssemblyTypes { get; set; } = new List<Type>();

    private Registry RegistryState { get; set; } = Registry.IDLE;

    private void Awake()
    {
        BuildRegistry();
    }

    public void NotifyObserverWrapper<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV3.IRequest<T> subject, Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifyObserver(context, subject, observer, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public IEnumerator NotifyObserver<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV3.IRequest<T> subject, Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        yield return null;
    }

    public IEnumerator NotifyObserver<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV4.IRequest<T> subject, Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        yield return null;
    }

    public IEnumerator NotifyObservers<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.Base.IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        yield return StartCoroutine(NotifyObservers<T>(context, (Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<T>)subject, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public IEnumerator NotifyObservers<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV2.IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        yield return StartCoroutine(NotifyObservers<T>(context, (Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<T>)subject, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public IEnumerator NotifyObservers<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV3.IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        yield return StartCoroutine(NotifyObservers<T>(context, (Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<T>)subject, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public IEnumerator NotifyObservers<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        yield return new WaitUntil(() => RegistryState.Equals(Registry.REGISTRY_READY));

        KeyValuePair<ISubjectBundle, List<IObserverBundle>> association = Associations.Where(kvp => kvp.Key.SubjectAttribute.SubjectType == context.EntityType).FirstOrDefault();

        if (association.Value == null)
        {
            throw new MissingContractException($"No observer found for the subject type: {association.Key.SubjectAttribute.SubjectType}!");
        }


        if (association.Key.Subject == null)
        {
            Debug.LogWarning($"The subject instance is null for the subject type: {context.EntityType}. Will update the dictionary with the current instance!");

            //check later if the casting will work seamlessly
            association.Key.Subject = (Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest) subject;
        }

        List<Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify> cachedObserverContext = GetObserverBundles<T, SubjectContext<T>> (association, context);

        if (cachedObserverContext == null || cachedObserverContext.Count == 0)
        {
            Debug.LogWarning($"The cached observers are null or either have not broadcasted their presence. Retrying...");

            yield return new WaitForSeconds(sleepTimeInMilliSeconds);

            yield return StartCoroutine(NotifyObservers<T>(context, subject, maxRetries - 1, sleepTimeInMilliSeconds, optional));
        }

        cachedObserverContext.ForEach(observer =>
        {
            //check later if the casting will work seamlessly
            Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<T> observerNotify = (Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<T>) observer;

            if (observerNotify == null)
            {
                throw new MissingContractException($"The observer instance does not implement the INotify<{typeof(T).Name}");
            }

            observerNotify.Notify(context.Data);
        });

        yield return null;
    }

    public IEnumerator NotifySubject<T>(ObserverContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        Debug.Log($"Before Registry State: {RegistryState}");

        yield return new WaitUntil(() => RegistryState.Equals(Registry.REGISTRY_READY));

        Debug.Log($"After Registry State: {RegistryState}");

        if (maxRetries == 0)
        {
            throw new MissingContextException($"Unable to fish for the subject type within the scene: {context.SubjectType}!");
        }

        Debug.Log($"Incoming Context: {context.ToString()}");

        if (context == null || context.SubjectType == null || context.Instance == null)
        {
            throw new MissingContextException($"Either the context is null or SubjectType/Instance are missing from the instance!");
        }

        KeyValuePair<ISubjectBundle, List<IObserverBundle>> association = Associations.Where(kvp => kvp.Key.SubjectAttribute.SubjectType == context.SubjectType).FirstOrDefault();

        Debug.Log($"Association: {association}");

        if (association.Value == null || association.Value.Count == 0)
        {
            throw new MissingContractException($"No observer found for the subject type: {context.SubjectType}!");
        }

        IObserverBundle cachedObserverContext = GetObserverBundle<T, ObserverContext>(association.Value, context);

        Debug.Log($"CachecObserverBundle: {cachedObserverContext}, Incoming Context: {context}, Type: {typeof(T)}");

        if (cachedObserverContext.Observer == null)
        {
            cachedObserverContext.Observer = (Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify) observer;
            Associations[association.Key].Add(cachedObserverContext);
        }

        if (association.Key.Subject == null)
        {
            Debug.LogWarning($"The subject instance is null for the subject type: {context.SubjectType}. Attemping a retry...");

            yield return new WaitForSeconds(sleepTimeInMilliSeconds);

            yield return StartCoroutine(NotifySubject<T>(context, observer, maxRetries - 1, sleepTimeInMilliSeconds, optional));
        }

        //see if its better to store it?? (compare letter the difference/performance)
        Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<T> subjectRequest = (Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<T>) association.Key.Subject;

        if (subjectRequest == null)
        {
            throw new MissingContractException($"The subject instance does not implement the IRequest<{typeof(T).Name}");
        }

        subjectRequest.Request();

        yield return null;
    }
    public IEnumerator NotifyObserver<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<T> subject, Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        yield return null;
    }

    public void NotifyObserversWrapper<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifyObservers(context, subject, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public void NotifyObserversWrapper<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV3.IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifyObservers(context, subject, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public void NotifyObserversWrapper<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.Base.IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifyObservers(context, subject, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public void NotifyObserversWrapper<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV2.IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifyObservers(context, (Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<T>) subject, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public void NotifySubjectWrapper<T>(ObserverContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifySubject(context, observer, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    //we should check on generic interface assignment since we wouldn't know concrete implementation during reflection.
    //in order to do that, get interfaces first and then check on IsGenericFlag and TypeDefinition
    public void BuildRegistry()
    {
        try
        {
            RegistryState = Registry.BUILDING_REGISTRY;

            Debug.Log($"Executing BuildRegistry...");

            ExecutingAssemblyTypes = Assembly.GetExecutingAssembly().GetTypes().ToArray().ToList();

            List<SubjectAttribute> subjects = Find<SubjectAttribute>(
                    ExecutingAssemblyTypes, new List<Type>() 
                    { 
                        typeof(Assets.Scripts.Interfaces.Mediator.Base.IRequest<>),
                        typeof(Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<>),
                        typeof(Assets.Scripts.Interfaces.Mediator.EnhancedV2.IRequest<>),
                        typeof(Assets.Scripts.Interfaces.Mediator.EnhancedV3.IRequest<>),
                        typeof(Assets.Scripts.Interfaces.Mediator.EnhancedV4.IRequest<>),

                    },
                    new List<Type>() 
                    { 
                        typeof(Assets.Scripts.Interfaces.Mediator.Base.IRequest),
                        typeof(Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest),
                    }
                ).ToList();

            Debug.Log($"Subjects found: {subjects.Count}");

            List<ObserverAttribute> observers = Find<ObserverAttribute>(
                     ExecutingAssemblyTypes, new List<Type> 
                     { 
                         typeof(Assets.Scripts.Interfaces.Mediator.Base.INotify<>),
                         typeof(Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<>),
                     },
                     new List<Type>
                     {
                         typeof(Assets.Scripts.Interfaces.Mediator.Base.INotify),
                         typeof(Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify)
                     }
                ).ToList();

            Debug.Log($"Observers found: {observers.Count}");

            foreach (SubjectAttribute subject in subjects)
            {
                ObserverBundle observerBundle = new ObserverBundle()
                {
                    ObserverAttribute = observers.Find(observer => observer.SubjectType.Equals(subject.SubjectType))
                };

                SubjectBundle subjectBundle = new SubjectBundle() { SubjectAttribute = subject };

                Debug.Log("SubjectBundle: " + subjectBundle.ToString() + " " + "Observerbundle: " + observerBundle.ToString());

                //check if exists - append, otherwise create a new list!!!
                if (!Associations.TryGetValue(subjectBundle, out List<IObserverBundle> observerBundles))
                {
                    Debug.Log($"Adding to association: {subjectBundle?.SubjectAttribute?.SubjectType} - {observerBundle?.ObserverAttribute?.ObserverType}");
                    Associations[subjectBundle] = new List<IObserverBundle>() { observerBundle };
                    continue;
                }

                Debug.Log($"Adding to association: {subjectBundle?.SubjectAttribute?.SubjectType} - {observerBundle?.ObserverAttribute?.ObserverType}");

                Associations[subjectBundle].Add(observerBundle);
            }

            Debug.Log("Done Registering...");

            RegistryState = Registry.REGISTRY_READY;
        }
        catch (BaseException ex)
        {
            Debug.Log($"Exception: {ex.Message}");
        }
    }

    private HashSet<T> Find<T>(List<Type> types, List<Type> genericInterfaceTypes = null, List<Type> nonGenericInterfaceTyles = null) where T : Attribute
    {
        if (genericInterfaceTypes == null && nonGenericInterfaceTyles == null)
        {
            throw new MissingArgumentException($"One of them must be provided : genericInterfaceTypes or nonGenericInterfaceTyles!");
        }

        HashSet<T> foundAttributes = new HashSet<T>();

        foreach (Type type in types)
        {
            List<T> attributes = type.GetCustomAttributes<T>().ToList();

            if (attributes == null || attributes.Count == 0)
            {
                Debug.Log($"No custom attribute found for type: {type.FullName}");

                continue;
            }

            string joinedGenericInterfaceTypes = string.Join<Type>(",", genericInterfaceTypes.ToArray());

            string joinedNonGenericInterfaceTypes = string.Join<Type>(",", nonGenericInterfaceTyles.ToArray());

            Debug.Log($"Custom attributes found for type: {type.FullName} - Count: {attributes.Count} - joinedGenericInterfaceTypes: {joinedGenericInterfaceTypes} - joinedNonGenericInterfaceTypes: {joinedNonGenericInterfaceTypes} - Total Interfaces: {type.GetInterfaces().Count()}");


            if (!type.GetInterfaces().Any(interf => genericInterfaceTypes.Any(possibleInterfaceType => interf.IsGenericType && possibleInterfaceType.GetGenericTypeDefinition() == interf.GetGenericTypeDefinition())))
            {
                throw new MissingContractException($"The underlying type must implement one of the interfaces: {joinedGenericInterfaceTypes}!");
            }

            attributes.ForEach(attribute =>
            {
                Debug.Log($"Adding: {attribute}");
               foundAttributes.Add(attribute);
            });
        }

        return foundAttributes;
    }

    private IObserverBundle GetObserverBundle<T, Z>(List<IObserverBundle> observers, Z context) where Z: ObserverContext
    {
        Debug.Log($"Getting observer bundle for context: {context}, Type: {typeof(T).Name}, Observer Count: {observers.Count}");
        return observers.Where(observerContext => observerContext.ObserverAttribute.ObserverType.Equals(context.EntityType) &&
                                                    typeof(T).Name.Equals(observerContext.ObserverAttribute.ContextType) && 
                                                    observerContext.ObserverAttribute.SubjectType.Equals(context.SubjectType)).FirstOrDefault();
    }

    private List<Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify> GetObserverBundles<T, Z>(KeyValuePair<ISubjectBundle, List<IObserverBundle>> association, Z context) where Z : SubjectContext<T>
    {
        return association.Value.Where(observerContext => observerContext.ObserverAttribute.SubjectType.Equals(context.EntityType) && typeof(T).Name.Equals(context.Data.GetType())).Select(observer => observer.Observer).ToList();
    }

    internal IEnumerator NotifyObservers(SubjectContext<IEntityTransform> subjectContext, PlayerAttributesNotifier playerAttributesNotifier)
    {
        throw new NotImplementedException();
    }
}