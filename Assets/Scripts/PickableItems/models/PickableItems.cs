
using System;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "PickableItems", menuName = "Scriptable Pickable Items")]
[Asset(AssetType = Asset.SCRIPTABLE_OBJECT, AddressLabel = "PickableItems")]
public class PickableItems : ScriptableObject, ISubject<IObserver<ScriptableObject>>
{
    private ScriptableObjectDelegator ScriptableObjectDelegator { get; set; }

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

    public void SetupAsSubject()
    {
        Debug.Log("OnEnable for PickableItems");

        ScriptableObjectDelegator = Helper.GetDelegator<ScriptableObjectDelegator>();

        Debug.Log($"Found the Scriptable Object Delegator: {ScriptableObjectDelegator}");

        Debug.Log("Adding to the AddToSubjectsDict");

        ScriptableObjectDelegator.AddToSubjectsDict(typeof(PickableItems).ToString(), typeof(PickableItems).ToString(), new Subject<IObserver<ScriptableObject>>());

        Debug.Log("Adding to the GetSubsetSubjectsDictionary");

        ScriptableObjectDelegator.GetSubsetSubjectsDictionary(typeof(PickableItems).ToString())[typeof(PickableItems).ToString()].SetSubject(this);
    }
}
