
using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PickableItems", menuName = "Scriptable Pickable Items")]
[Asset(Asset.SCRIPTABLE_OBJECT, "PickableItems", InstantiationOrder = 2)]
[Subject(AssetType = Asset.SCRIPTABLE_OBJECT, SubjectType = typeof(PickableItems), ContextType = typeof(ScriptableObject))]
public class PickableItems : ScriptableObject, IRequest<ScriptableObject>, IDelegate
{
    private Delegator Delegator { get; set; }
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

    public async void SetupAsSubject()
    {
        Delegator = await Helper.GetDelegator<Delegator>();
    }

    public IEnumerator Request()
    {
       Delegator.NotifyObserversWrapper(new SubjectContext<ScriptableObject>()
        {
            EntityType = typeof(PickableItems),
            Data = (PickableItems)this

        }, this);

        yield return null;
    }
}
