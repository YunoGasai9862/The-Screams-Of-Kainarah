using GlobalAccessAndGameHelper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using InventoryManagement = CreateInventorySystem;

public class PlayerActionSystemHandler : MonoBehaviour, IObserver<Collider2D>
{
    [SerializeField] PickableItemsClass pickableItems;

    Dictionary<String, Func<Collider2D, Task >> _playerActionHandlerDic;

    private GameObjectInstantiator _gameObject;

    private void Awake()
    {

        _playerActionHandlerDic = new Dictionary<String, Func<Collider2D, Task>>
        {

             { "Crystal", value => OnCrystalPickup(value)},
             { "Health" , value => OnHealthPickup(value) },
             { "Dagger" , value => OnDaggerPickup(value) }

        };

    }

    private async Task<bool> OnDaggerPickup(Collider2D collider)
    {
        GameObject temp = pickableItems.returnGameObjectForTheKey(collider.tag);

        return await InventoryManagement.AddToInventorySystem(temp.GetComponent<SpriteRenderer>().sprite, temp.tag); //adds it to the inventory

    }

    private async Task<bool> OnHealthPickup(Collider2D collider)
    {
        Vector2 _pickupPos = new(collider.transform.position.x, collider.transform.position.y - 1f);
        GameObjectInstantiator _gameObject = pickupEffectInstantiator(pickableItems.returnGameObjectForTheKey(collider.tag), _pickupPos);
        _gameObject.DestroyGameObject(3f);
        return await Task.FromResult(true);

    }

    private async Task<bool> OnCrystalPickup(Collider2D collider)
    {
        pickupEffectInstantiator(pickableItems.returnGameObjectForTheKey(collider.tag), collider.transform.position);
        return await Task.FromResult(true);
    }

    private void OnEnable()
    {
        PlayerObserverListenerHelper.ColliderSubjects.AddObserver(this); //Add PlayerActionSystem as an observer
    }

    private void OnDisable()
    {
        PlayerObserverListenerHelper.ColliderSubjects.RemoveOberver(this); //Remove PlayerActionSystem as an observer when an event is handled/or the observer is no longer needed
    }
    private GameObjectInstantiator pickupEffectInstantiator(GameObject prefab, Vector3 position)
    {
        _gameObject = new(prefab);
        _gameObject.InstantiateGameObject(position, Quaternion.identity);
        return _gameObject;
    }

    public void OnNotify(ref Collider2D collider, params object[] optional)
    {
        if(_playerActionHandlerDic.TryGetValue(collider.tag, out var invokeFunc)) //simplified
        {
            invokeFunc.Invoke(collider);
        }
    }
}
