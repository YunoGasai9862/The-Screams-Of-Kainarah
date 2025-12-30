using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;
using Assets.Annotations;

public class BaseDelegator : MonoBehaviour, IDelegator
{
    private Dictionary<SubjectAttribute, List<ObserverAttribute>> SubjectObserverAssociationsDict { get; set; }

    public IEnumerator NotifyObserver(dynamic value, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, params object[] optional)
    {
        yield return null;
    }

    public IEnumerator NotifySubject(NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim = null, int maxRetries = 3, int sleepTimeInMilliSeconds = 3000, params object[] optional)
    {
        if (maxRetries == 0)
        {
            throw new ApplicationException($"No such subject type exists! - Please Register first {notificationContext.SubjectType}. Seeker: {observer}");
        }

        if (notificationContext.SubjectType == null)
        {
            throw new ApplicationException($"Subject type is null - please add it in the notification context object!");
        }

        //yield return new WaitUntil(() => !Helper.IsObjectNull(SubjectsDict));

        //if (SubjectObserverAssociationsDict.TryGetValue(notificationContext.SubjectType, out Dictionary<string,Subject<T>> subjects))
        //{
        //    foreach (KeyValuePair<string,Subject<T>> keyValuePair in subjects)
        //    {
        //        yield return new WaitUntil(() => !Helper.IsSubjectNull(keyValuePair.Value));

        //        keyValuePair.Value.NotifySubject(observer, notificationContext, cancellationToken);
        //    }
        //}
        //else
        //{
        //    yield return new WaitForSeconds(Helper.GetSecondsFromMilliSeconds(sleepTimeInMilliSeconds));

        //    Debug.Log($"Retrying for - {notificationContext.SubjectType} / Seeker: {observer} length of the dict: {SubjectsDict.Count} - retries left: {maxRetries}");

        //    StartCoroutine(NotifySubject(observer, notificationContext, cancellationToken, semaphoreSlim, maxRetries -= 1, sleepTimeInMilliSeconds, optional));
        //}
        

        yield return null;
    }

    public void BuildRegistry()
    {

    }
}
