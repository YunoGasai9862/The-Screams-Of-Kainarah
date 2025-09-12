
using System;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "PickableItems", menuName = "Scriptable Pickable Items")]
[Asset(AssetType = Asset.SCRIPTABLE_OBJECT, AddressLabel = "PickableItems")]
public class PickableItems : ScriptableObject, ISubject<IObserver<ScriptableObject>>, IDelegate
{
    private ScriptableObjectDelegator ScriptableObjectDelegator { get; set; }
    public IDelegate.InvokeMethod InvokeCustomMethod { get; set; }

    private void OnEnable()
    {
        InvokeCustomMethod += SetupAsSubject;
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

    public void SetupAsSubject()
    {
        ScriptableObjectDelegator = Helper.GetDelegator<ScriptableObjectDelegator>();

        ScriptableObjectDelegator.AddToSubjectsDict(typeof(PickableItems).ToString(), typeof(PickableItems).ToString(), new Subject<IObserver<ScriptableObject>>());

        ScriptableObjectDelegator.GetSubsetSubjectsDictionary(typeof(PickableItems).ToString())[typeof(PickableItems).ToString()].SetSubject(this);
    }
}
