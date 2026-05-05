using Annotations.Enums;
using Assets.Annotations;
using Assets.Annotations.Interfaces;
using Assets.Exceptions;
using Assets.Scripts.Enums;
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
    private Dictionary<dynamic, List<dynamic>> Associations { get; set; } = new Dictionary<dynamic, List<dynamic>>();

    private List<Type> ExecutingAssemblyTypes { get; set; } = new List<Type>();

    private Registry RegistryState { get; set; } = Registry.IDLE;

    private void Awake()
    {
        BuildDelegatorRegistry();
    }

    public void NotifyObserverWrapper<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV3.IRequest<T> subject, Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifyObserver<T>(context, subject, observer, maxRetries, sleepTimeInMilliSeconds, optional));
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

        KeyValuePair<dynamic, List<dynamic>> association = Associations.Where(kvp => kvp.Key.SubjectAttribute.EntityType == context.EntityType).FirstOrDefault();

        if (association.Value == null)
        {
            throw new MissingContractException($"No observer found for the subject type: {association.Key.SubjectAttribute.EntityType}!");
        }


        if (association.Key.Subject == null)
        {
            Debug.LogWarning($"The subject instance is null for the subject type: {context.EntityType}. Will update the dictionary with the current instance!");

            //check later if the casting will work seamlessly
            association.Key.Subject = subject;
        }

        if (!IsValid(association.Key, context))
        {
            Debug.Log($"The subject: {association.Key} is not valid.");

            yield return new WaitForSeconds(sleepTimeInMilliSeconds / 1000);

            yield return StartCoroutine(NotifyObservers<T>(context, subject, maxRetries - 1, sleepTimeInMilliSeconds, optional));
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
        yield return new WaitUntil(() => RegistryState.Equals(Registry.REGISTRY_READY));

        if (maxRetries == 0)
        {
            context?.FallBack?.Alert.Invoke();

            throw new MissingContextException($"Unable to fish for the subject type within the scene: {context.SubjectType}!");
        }


        if (context == null || context.SubjectType == null || context.EntityType == null)
        {
            throw new MissingContextException($"Either the context is null or SubjectType/EntityType are missing from the context!");
        }

        KeyValuePair<dynamic, List<dynamic>> association = Associations.Where(kvp => kvp.Key.SubjectAttribute.EntityType == context.SubjectType).FirstOrDefault();

        if (association.Value == null || association.Value.Count == 0)
        {
            throw new MissingContractException($"No observer found for the subject type: {context.SubjectType}!");
        }

        IObserverBundle cachedObserverContext = GetObserverBundle<T, ObserverContext>(association.Value, context);

        if (!IsValid(cachedObserverContext.ObserverAttribute, context))
        {
            Debug.Log($"The observer: {cachedObserverContext.Observer} is not valid for the context: {context}! Retrying in case if it's a stale reference...");

            yield return new WaitForSeconds(sleepTimeInMilliSeconds / 1000);

            yield return StartCoroutine(NotifySubject<T>(context, observer, maxRetries - 1, sleepTimeInMilliSeconds, optional));
        }

        Debug.Log($"CachedObserverBundle: {cachedObserverContext}, Incoming Context: {context}, Type: {typeof(T)}, observer: {observer}");

        if (cachedObserverContext?.Observer == null)
        {
            Debug.Log($"The observer instance is null for the context: {context}. Association Subject: {association.Key}, Creating a new observer bundle...");

            ObserverBundle<T> observerBundle = new ObserverBundle<T>()
            {
                Observer = observer,
                ObserverAttribute = cachedObserverContext.ObserverAttribute
            };

            Associations[association.Key].Add(observerBundle);
        }

        if (association.Key.Subject == null)
        {
            Debug.Log($"The subject instance could not be found for the subject type: {context.SubjectType}. Trying to find in the scene");

            GameObject gameObject = FindObjectOfType(context.SubjectType) as GameObject;

            Debug.Log($"In Scene: {gameObject} - type: {context.SubjectType}");

            if (gameObject == null)
            {
                Debug.Log($"GameObject - {gameObject} not found, retrying...");

                yield return new WaitForSeconds(sleepTimeInMilliSeconds / 1000);

                yield return StartCoroutine(NotifySubject<T>(context, observer, maxRetries - 1, sleepTimeInMilliSeconds, optional));
            }

            if (!Helper.IsInterfacePresent(gameObject, typeof(Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest)))
            {
                Debug.Log($"The subject instance does not implement the IRequest interface for the subject type: {context.SubjectType}. Exiting...");

                yield return null;
            }

            Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest subjectInstance = gameObject.GetComponent<Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest>(); 

            if (subjectInstance == null)
            {
                Debug.Log($"EnhancedV1.Request could not be found for: {context.SubjectType}. Retrying...");

                yield return new WaitForSeconds(sleepTimeInMilliSeconds / 1000);

                yield return StartCoroutine(NotifySubject<T>(context, observer, maxRetries - 1, sleepTimeInMilliSeconds, optional));
            }

            Associations.Remove(association.Key);

            KeyValuePair<dynamic, List<dynamic>> updatedAssociation = new KeyValuePair<dynamic, List<dynamic>>(
                new SubjectBundle() { SubjectAttribute = association.Key.SubjectAttribute, Subject = subjectInstance },
                association.Value
            );

            Associations.Add(updatedAssociation.Key, updatedAssociation.Value);

            //it must point to this new association since the key has been updated with the subject instance
            association = updatedAssociation;
        }

        Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<T> subjectRequest = (Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<T>) association.Key.Subject;

        Debug.Log($"SubjectRequest: {subjectRequest}, SubjectInstance: {association.Key.Subject}, Type: {typeof(T)}, observer: {observer}");

        if (subjectRequest == null)
        {
            throw new MissingContractException($"The subject instance does not implement the IRequest<{typeof(T).Name}");
        }

        subjectRequest.Request();

        yield return null;
    }

    private bool IsValid<Z, W>(W data, Z context = null) where W: IData where Z: Context
    {
        switch (data.AssetType)
        {
            case Asset.NONE:
                return false;

            case Asset.MONOBEHAVIOR:
                Debug.Log($"IData(MonoBehavior) - data:: {typeof(MonoBehaviour).IsAssignableFrom(data.EntityType)}");
                bool isMonoBehavior = typeof(MonoBehaviour).IsAssignableFrom(data.EntityType);
                return context == null ? isMonoBehavior : isMonoBehavior && context.Instance != null;

            case Asset.SCRIPTABLE_OBJECT:
                return typeof(ScriptableObject).IsAssignableFrom(data.EntityType);

            case Asset.PLAYER_STATE_MACHINE:
                return typeof(StateMachineBehaviour).IsAssignableFrom(data.EntityType);
        }

        return false;
    }

    public IEnumerator NotifyObserver<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<T> subject, Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        yield return null;
    }

    public void NotifyObserversWrapper<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifyObservers<T>(context, subject, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public void NotifyObserversWrapper<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV3.IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifyObservers<T>(context, subject, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public void NotifyObserversWrapper<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.Base.IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifyObservers<T>(context, subject, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public void NotifyObserversWrapper<T>(SubjectContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV2.IRequest<T> subject, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifyObservers<T>(context, (Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<T>) subject, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    public void NotifySubjectWrapper<T>(ObserverContext<T> context, Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<T> observer, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        StartCoroutine(NotifySubject(context, observer, maxRetries, sleepTimeInMilliSeconds, optional));
    }

    //we should check on generic interface assignment since we wouldn't know concrete implementation during reflection.
    //in order to do that, get interfaces first and then check on IsGenericFlag and TypeDefinition
    public void BuildDelegatorRegistry()
    {
        try
        {
            RegistryState = Registry.BUILDING_REGISTRY;

            Debug.Log($"Executing BuildRegistry...");

            ExecutingAssemblyTypes = Assembly.GetExecutingAssembly().GetTypes().ToArray().ToList();

            List<SubjectAttribute> subjects = Helper.GetAttribute<SubjectAttribute>(
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

            List<ObserverAttribute> observers = Helper.GetAttribute<ObserverAttribute>(
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
                if (subject.AssetType == Asset.NONE || subject.EntityType == null || subject.ContextType == null)
                {
                    Debug.LogWarning($"Either AssetType, SubjectType or ContextType is missing for the subject attribute: {subject}. Skipping the registration for this subject!");
                    continue;
                }

                SubjectBundle subjectBundle = new SubjectBundle() { SubjectAttribute = subject };

                List<ObserverAttribute> specificObservers = observers.Where(observer => observer.SubjectType.Equals(subject.EntityType)).ToList();

                foreach (ObserverAttribute observer in specificObservers)
                {
                    if (observer.AssetType == Asset.NONE || observer.SubjectType == null || observer.ContextType == null || observer.EntityType == null)
                    {
                        Debug.LogWarning($"Either AssetType, SubjectType, ContextType, or Observertype is missing for the observer attribute: {observer}. Skipping the registration for this observer!");
                        continue;
                    }

                    ObserverBundle observerBundle = new ObserverBundle()
                    {
                        ObserverAttribute = observer
                    };

                    Debug.Log("SubjectBundle: " + subjectBundle.ToString() + " " + "Observerbundle: " + observerBundle.ToString());

                    //check if exists - append, otherwise create a new list!!!
                    if (!Associations.TryGetValue(subjectBundle, out List<dynamic> observerBundles))
                    {
                        Debug.Log($"Adding to association: {subjectBundle?.SubjectAttribute?.EntityType} - {observerBundle?.ObserverAttribute?.EntityType}");
                        Associations[subjectBundle] = new List<dynamic>() { observerBundle };
                        continue;
                    }

                    Debug.Log($"Adding to association: {subjectBundle?.SubjectAttribute?.EntityType} - {observerBundle?.ObserverAttribute?.EntityType}");

                    Associations[subjectBundle].Add(observerBundle);
                }
            }

            Debug.Log("Done Registering...");

            RegistryState = Registry.REGISTRY_READY;
        }
        catch (BaseException ex)
        {
            Debug.Log($"Exception: {ex.Message}");
        }
    }

    //System.Runtime.Exception ==> Reflection.TypeInfo doesnot contain Equals definition
    private IObserverBundle GetObserverBundle<T, Z>(List<dynamic> observers, Z context) where Z: ObserverContext
    {
        foreach (var observer in observers)
        {
            Debug.Log($"Observer: {observer}, ObserverAttribute: {observer.ObserverAttribute}, Context: {context}, Type: {typeof(T)}");
        }

        IObserverBundle value = observers.Where(observerContext => observerContext.ObserverAttribute.EntityType == context.EntityType &&
                                                    typeof(T) == observerContext.ObserverAttribute.ContextType && 
                                                    observerContext.ObserverAttribute.SubjectType == context.SubjectType).First();

        return value;


    }

    private List<Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify> GetObserverBundles<T, Z>(KeyValuePair<dynamic, List<dynamic>> association, Z context) where Z : SubjectContext<T>
    {
        return association.Value.Where(observerContext => observerContext.ObserverAttribute.SubjectType == context.EntityType && typeof(T).Name.Equals(context.Data.GetType())).Select(observer => (Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify)observer.Observer).ToList();
    }
}