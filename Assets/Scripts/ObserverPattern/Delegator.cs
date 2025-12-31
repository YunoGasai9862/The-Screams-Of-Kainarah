using Assets.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
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

    //CHECK HOW WILL YOU USE ON NOTIFY NOW!!!
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

                if (observerAttribute == null)
                {
                    Debug.Log($"No ObserverAttribute found for type: {type.FullName}");
                    continue;
                }

                SubjectAttribute subjectAttribute = type.GetCustomAttribute<SubjectAttribute>();

                if (subjectAttribute == null)
                {
                    Debug.Log($"No SubjectAttribute found for type: {type.FullName}");
                    continue;
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