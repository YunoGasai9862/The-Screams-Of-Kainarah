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
        if (Subjects.TryGetValue(context.EntityType.ToString(), out List<SubjectAttribute> subjects))
        {

        }

        yield return null;
    }

    public IEnumerator NotifySubject<T>(ObserverContext<T> context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        if (context == null || context.Name == null || context.Tag == null || context.SubjectType == null)
        {
            throw new MissingContextException($"Either the context is null or name/tag/SubjectType are missing from the instance!");
        }

        //once the dictionary has been built, we need to query live instances to make sure what observer is claiming, really exists!!
        List<ObserverAttribute> targetObservers = Observers.Keys.Where(key => key == context.SubjectType.ToString()).Select(key => Observers[key]).FirstOrDefault().ToList();

        targetObservers.ForEach(observer =>
        {

        });

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

            foreach (Type type in types)
            {
                ObserverAttribute observerAttribute = type.GetCustomAttribute<ObserverAttribute>();

                SubjectAttribute subjectAttribute = type.GetCustomAttribute<SubjectAttribute>();

                if (subjectAttribute == null && observerAttribute == null)
                {
                    Debug.Log($"No SubjectAttribute & ObserverAttribute found for type: {type.FullName}");
                    continue;
                }

                if (observerAttribute != null && type.GetInterfaces().Any(interf => interf.IsGenericType && interf.GetGenericTypeDefinition() == typeof(INotify<>)))
                {
                    throw new MissingContractException("Observer must implement INotify<*>!");
                }

                if (subjectAttribute != null && type.GetInterfaces().Any(interf => interf.IsGenericType && interf.GetGenericTypeDefinition() == typeof(IRequest<>)))
                {
                    throw new MissingContractException("Subject must implement IRequest<*>!");
                }

                PopulateDictionary(Observers, observerAttribute, observerAttribute.SubjectType.FullName);

                PopulateDictionary(Subjects, subjectAttribute, subjectAttribute.SubjectType.FullName);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    private void InjectSubjects()
    {

    }

    private void InjectObservers()
    {

    }
}