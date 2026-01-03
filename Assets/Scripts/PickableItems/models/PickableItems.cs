
using System;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "PickableItems", menuName = "Scriptable Pickable Items")]
[Asset(Asset.SCRIPTABLE_OBJECT, "PickableItems", InstantiationOrder = 2)]
public class PickableItems : ScriptableObject, ISubject<ScriptableObject>, IDelegate
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

    public void OnNotifySubject(IObserver<ScriptableObject> observer, Context context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        ScriptableObjectDelegator.NotifyObjectWrapper(observer, (PickableItems) this, new Context()
        {
            EntityType = typeof(PickableItems).ToString(),

        }, CancellationToken.None);
    }

    public async void SetupAsSubject()
    {
        ScriptableObjectDelegator = await Helper.GetDelegator<ScriptableObjectDelegator>();

        ScriptableObjectDelegator.AddToSubjectsDict(typeof(PickableItems).ToString(), typeof(PickableItems).ToString(), new Subject<ScriptableObject>(this, typeof(ScriptableObject)));
    }
}
