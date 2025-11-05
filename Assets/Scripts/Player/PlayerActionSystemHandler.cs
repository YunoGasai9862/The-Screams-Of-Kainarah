using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerActionSystemHandler : MonoBehaviour, IObserver<Collider2D>, IObserver<ScriptableObject>
{
    [SerializeField] PlayerPowerUpModeEvent playerPowerUpModeEvent;
    [SerializeField] CrystalUIIncrementEvent crystalUIIncrementEvent;

    private Dictionary<String, Func<Collider2D, Task >> _playerActionHandlerDic;
    private PickableItemsUtility PickableItemsUtility { get; set; }

    private InstantiateUtility InstantiateUtilityInstannce { get; set; } = new InstantiateUtility();

    private ScriptableObjectDelegator ScriptableObjectDelegator { get; set; }
    private float DIAMOND_PICK_UP_VALUE { get; set; } = 20f;
    private int CRYSTAL_UI_INCREMENT_VALUE { get; set; } = 1;

    private async void Awake()
    {
        ScriptableObjectDelegator = await Helper.GetDelegator<ScriptableObjectDelegator>();

        _playerActionHandlerDic = new Dictionary<String, Func<Collider2D, Task>>
        {
             { "Crystal", value => OnCrystalPickup(value)},
             { "Health" , value => OnHealthPickup(value) },
             { "Dagger" , value => OnDaggerPickup(value) }
        };

        StartCoroutine(ScriptableObjectDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = name,
            ObserverTag = tag,
            SubjectType = typeof(PickableItems).ToString()

        }, CancellationToken.None));
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

    public void OnNotify(Collider2D data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        if (_playerActionHandlerDic.TryGetValue(data.tag, out var invokeFunc)) //simplified
        {
            invokeFunc.Invoke(data);
        }
    }

    public void OnNotify(ScriptableObject data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PickableItemsUtility = new PickableItemsUtility((PickableItems)data);

    }
}
