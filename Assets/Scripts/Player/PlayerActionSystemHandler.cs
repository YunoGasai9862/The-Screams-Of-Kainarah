using Assets.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Threading.Tasks;
using UnityEngine;
using Annotations.Enums;

[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(PlayerActionSystemHandler), EntityType = typeof(PickableItems), ContextType = typeof(Collider2D))]
[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(PlayerActionSystemHandler), EntityType = typeof(EntityPoolManager), ContextType = typeof(EntityPoolManager))]
public class PlayerActionSystemHandler : MonoBehaviour, INotify<Collider2D>, INotify<EntityPoolManager>
{
    [SerializeField] PlayerPowerUpModeEvent playerPowerUpModeEvent;
    [SerializeField] CrystalUIIncrementEvent crystalUIIncrementEvent;

    private Dictionary<String, Func<Collider2D, Task >> _playerActionHandlerDic;

    private const string PICKABLE_ITEMS_KEY = "PickableItems";

    private PickableItemsUtility PickableItemsUtility { get; set; }

    private EntityPoolManager EntityPoolManagerInstance { get; set; }

    private PickableItems PickableItemsSO { get; set; }

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

    public IEnumerator Notify(EntityPoolManager value)
    {
        EntityPoolManagerInstance = value;

        PickableItemsSO = Helper.GetFromEntityPoolManager<PickableItems>(EntityPoolManagerInstance, PICKABLE_ITEMS_KEY);

        PickableItemsUtility = new PickableItemsUtility(PickableItemsSO);

        yield return null;
    }
}
