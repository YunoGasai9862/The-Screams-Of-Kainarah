using Assets.Annotations;
using Assets.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class Delegator : MonoBehaviour, IDelegator
{
    private Dictionary<string, List<ObserverAttribute>> Observers { get; set; } = new Dictionary<string, List<ObserverAttribute>>();

    private Dictionary<string, List<SubjectAttribute>> Subjects { get; set; } = new Dictionary<string, List<SubjectAttribute>>();

    private void Awake()
    {
        BuildRegistry();
    }

    public IEnumerator NotifyObserver(dynamic value, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        yield return null;
    }

    public void BuildRegistry()
    {
        try
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in types)
            {
                ObserverAttribute observerAttribute = type.GetCustomAttribute<ObserverAttribute>();

                SubjectAttribute subjectAttribute = type.GetCustomAttribute<SubjectAttribute>();

                if (subjectAttribute == null && observerAttribute == null)
                {
                    Debug.Log($"No SubjectAttribute & ObserverAttribute found for type: {type.FullName}");
                    continue;
                }

                //we should check on generic interface assignment since we wouldn't know concrete implementation during reflection.
                //in order to do that, get interfaces first and then check on IsGenericFlag and TypeDefinition
                if (observerAttribute != null && type.GetInterfaces().Any(interf => interf.IsGenericType && interf.GetGenericTypeDefinition() == typeof(INotify<>)))
                {
                    throw new MissingContractException("Observer must implement the INotify!");
                }

                Observers[observerAttribute.SubjectType.FullName].Add(observerAttribute);

                Subjects[subjectAttribute.SubjectType.FullName].Add(subjectAttribute);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
}