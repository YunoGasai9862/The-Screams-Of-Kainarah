
using System;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "PickableItems", menuName = "Scriptable Pickable Items")]
public class PickableItems : ScriptableObject, ISubject<IObserver<ScriptableObject>>
{
    private ScriptableObjectDelegator ScriptableObjectDelegator { get; set; }

    private void OnEnable()
    {
        ScriptableObjectDelegator = Helper.GetDelegator<ScriptableObjectDelegator>();

        ScriptableObjectDelegator.AddToSubjectsDict(typeof(PickableItems).ToString(), name, new Subject<IObserver<ScriptableObject>>());

        ScriptableObjectDelegator.GetSubsetSubjectsDictionary(typeof(PickableItems).ToString())[name].SetSubject(this);

        Debug.Log($"Found the Scriptable Object Delegator: {ScriptableObjectDelegator}");
    }

    [Serializable]
    public class PickableEntities
    {
        public string objectName;
        public GameObject prefabToInstantiate;
        public bool shouldBeDisabledAfterSomeTime;
    }

    public PickableEntities[] pickableEntities;

    public void OnNotifySubject(IObserver<ScriptableObject> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        ScriptableObjectDelegator.NotifyObjectWrapper(observer, (PickableItems) this, new NotificationContext()
        {
            SubjectType = typeof(PickableItems).ToString(),

        }, CancellationToken.None);
    }
}
