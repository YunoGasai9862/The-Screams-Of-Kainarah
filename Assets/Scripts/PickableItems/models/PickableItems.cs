
using NUnit.Framework.Constraints;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "PickableItems", menuName = "Scriptable Pickable Items")]
public class PickableItems : ScriptableObject, ISubject<IObserver<ScriptableObject>>
{
    private ScriptableObjectDelegator ScriptableObjectDelegator { get; set; }

    private void Awake()
    {
        Debug.Log($"Pickable Items Awake!");

        ScriptableObjectDelegator = Helper.GetDelegator<ScriptableObjectDelegator>();
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
        //CHECK IF THIS WILL WORK OTHERWISE ALSO CREATE A SEPARATE DELEGATOR FOR PICKABLE ITEMS (CASTING ISSUES)
        ScriptableObjectDelegator.NotifyObjectWrapper(observer, (PickableItems) this, new NotificationContext()
        {
            SubjectType = typeof(PickableItems).ToString(),

        }, CancellationToken.None);
    }
}
