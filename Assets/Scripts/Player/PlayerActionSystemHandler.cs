using Assets.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Observer(ObserverType = typeof(PlayerActionSystemHandler), SubjectType = typeof(PickableItems), ContextType = typeof(Collider2D))]
[Observer(ObserverType = typeof(PlayerActionSystemHandler), SubjectType = typeof(PickableItems), ContextType = typeof(ScriptableObject))]
public class PlayerActionSystemHandler : MonoBehaviour, INotify<Collider2D>, INotify<ScriptableObject>
{
    [SerializeField] PlayerPowerUpModeEvent playerPowerUpModeEvent;
    [SerializeField] CrystalUIIncrementEvent crystalUIIncrementEvent;

    private Dictionary<String, Func<Collider2D, Task >> _playerActionHandlerDic;
    private PickableItemsUtility PickableItemsUtility { get; set; }

    private InstantiateUtility InstantiateUtilityInstannce { get; set; } = new InstantiateUtility();

    private Delegator Delegator { get; set; }
    private float DIAMOND_PICK_UP_VALUE { get; set; } = 20f;
    private int CRYSTAL_UI_INCREMENT_VALUE { get; set; } = 1;

    private async void Awake()
    {
        Delegator = await Helper.GetDelegator<Delegator>();

        _playerActionHandlerDic = new Dictionary<String, Func<Collider2D, Task>>
        {
             { "Crystal", value => OnCrystalPickup(value)},
             { "Health" , value => OnHealthPickup(value) },
             { "Dagger" , value => OnDaggerPickup(value) }
        };

        Delegator.NotifySubjectWrapper(new ObserverContext<ScriptableObject>()
        {
            Instance = gameObject,
            SubjectType = typeof(PickableItems)

        }, this);
    }
    private Task<bool> OnDaggerPickup(Collider2D collider)
    {
        GameObject temp = PickableItemsUtility.GetGameObject(collider.tag);
        InventoryManagementSystem.Instance.AddInvoke(temp.GetComponent<SpriteRenderer>().sprite, temp.tag);
        return Task.FromResult(true); //adds it to the inventory

    }

    private async Task<bool> OnHealthPickup(Collider2D collider)
    {
        Vector2 _pickupPos = new(collider.transform.position.x, collider.transform.position.y - 1f);

        InstantiateUtilityInstannce.SetPrefab(PickableItemsUtility.GetGameObject(collider.tag));

        InstantiateUtilityInstannce.InstantiateObject(_pickupPos, Quaternion.identity);

        InstantiateUtilityInstannce.DestroyObjectAfter(3f);

        return await Task.FromResult(true);

    }

    private async Task<bool> OnCrystalPickup(Collider2D collider)
    {
        InstantiateUtilityInstannce.SetPrefab(PickableItemsUtility.GetGameObject(collider.tag));

        InstantiateUtilityInstannce.InstantiateObject(collider.transform.position, Quaternion.identity);

        playerPowerUpModeEvent.GetInstance().Invoke(DIAMOND_PICK_UP_VALUE);

        await collider.GetComponent<MoveCrystal>().crystalCollideEvent.Invoke(collider, true);

        await InvokeCrystalUIEvent(crystalUIIncrementEvent, CRYSTAL_UI_INCREMENT_VALUE);

        return await Task.FromResult(true);
    }

    private Task InvokeCrystalUIEvent(CrystalUIIncrementEvent crystalUIIncrementEvent, int crystalValue)
    {
        crystalUIIncrementEvent.Invoke(crystalValue);

        return Task.CompletedTask;
    }

    private void OnEnable()
    {
        PlayerObserverListenerHelper.ColliderSubjects.AddObserver(this); //Add PlayerActionSystem as an observer
    }

    private void OnDisable()
    {
        PlayerObserverListenerHelper.ColliderSubjects.RemoveOberver(this); //Remove PlayerActionSystem as an observer when an event is handled/or the observer is no longer needed
    }

    public void OnNotify(Collider2D data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
      
    }

    public IEnumerator Notify(Collider2D value)
    {
        if (_playerActionHandlerDic.TryGetValue(value.tag, out var invokeFunc))
        {
            invokeFunc.Invoke(value);
        }

        yield return null;
    }

    public IEnumerator Notify(ScriptableObject value)
    {
        PickableItemsUtility = new PickableItemsUtility((PickableItems)value);

        yield return null;
    }
}
