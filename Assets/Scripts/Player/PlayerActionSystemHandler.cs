using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerActionSystemHandler : MonoBehaviour, IObserver<Collider2D>
{
    [SerializeField] PickableItemsHandler pickableItems;
    [SerializeField] PlayerPowerUpModeEvent playerPowerUpModeEvent;
    [SerializeField] CrystalUIIncrementEvent crystalUIIncrementEvent;

    Dictionary<String, Func<Collider2D, Task >> _playerActionHandlerDic;

    private InstantiatorController _gameObject;
    private float DIAMOND_PICK_UP_VALUE { get; set; } = 20f;
    private int CRYSTAL_UI_INCREMENT_VALUE { get; set; } = 1;

    private void Awake()
    {
        _playerActionHandlerDic = new Dictionary<String, Func<Collider2D, Task>>
        {
             { "Crystal", value => OnCrystalPickup(value)},
             { "Health" , value => OnHealthPickup(value) },
             { "Dagger" , value => OnDaggerPickup(value) }
        };
    }
    private Task<bool> OnDaggerPickup(Collider2D collider)
    {
        GameObject temp = pickableItems.ReturnGameObjectForTheKey(collider.tag);
        InventoryManagementSystem.Instance.AddInvoke(temp.GetComponent<SpriteRenderer>().sprite, temp.tag);
        return Task.FromResult(true); //adds it to the inventory

    }

    private async Task<bool> OnHealthPickup(Collider2D collider)
    {
        Vector2 _pickupPos = new(collider.transform.position.x, collider.transform.position.y - 1f);
        InstantiatorController _gameObject = pickupEffectInstantiator(pickableItems.ReturnGameObjectForTheKey(collider.tag), _pickupPos);
        _gameObject.DestroyGameObject(3f);
        return await Task.FromResult(true);

    }

    private async Task<bool> OnCrystalPickup(Collider2D collision)
    {
       pickupEffectInstantiator(pickableItems.ReturnGameObjectForTheKey(collision.tag), collision.transform.position);
       playerPowerUpModeEvent.GetInstance().Invoke(DIAMOND_PICK_UP_VALUE);
       await collision.GetComponent<MoveCrystal>().crystalCollideEvent.Invoke(collision, true);
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
    private InstantiatorController pickupEffectInstantiator(GameObject prefab, Vector3 position)
    {
        _gameObject = new(prefab);
        _gameObject.InstantiateGameObject(position, Quaternion.identity);
        return _gameObject;
    }

    public void OnNotify(Collider2D data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        if (_playerActionHandlerDic.TryGetValue(data.tag, out var invokeFunc)) //simplified
        {
            invokeFunc.Invoke(data);
        }
    }
}
