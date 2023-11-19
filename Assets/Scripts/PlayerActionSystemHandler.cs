using GlobalAccessAndGameHelper;
using System;
using System.Collections.Generic;
using UnityEngine;
using InventoryManagement = CreateInventorySystem;

public class PlayerActionSystemHandler : MonoBehaviour, IObserver<Collider2D>
{
    [SerializeField] PickableItemsClass pickableItems;

    Dictionary<String, System.Action> _playerActionHandlerDic;

    private GameObjectInstantiator _gameObject;

    private Collider2D _passedOnCollider;

    private void Awake()
    {

        _playerActionHandlerDic = new Dictionary<String, System.Action>
        {

             { "Crystal", OnCrystalPickup },
             { "Health" , OnHealthPickup  },
             { "Dagger" , OnDaggerPickup  }

        };

    }

    private void OnDaggerPickup()
    {
        GameObject temp = pickableItems.returnGameObjectForTheKey(_passedOnCollider.tag);

        InventoryManagement.AddToInventory(temp.GetComponent<SpriteRenderer>().sprite, temp.tag); //adds it to the inventory

    }

    private void OnHealthPickup()
    {
        Vector2 _pickupPos = new(_passedOnCollider.transform.position.x, _passedOnCollider.transform.position.y - 1f);
        GameObjectInstantiator _gameObject = pickupEffectInstantiator(pickableItems.returnGameObjectForTheKey(_passedOnCollider.tag), _pickupPos);
        _gameObject.DestroyGameObject(3f);

    }

    private void OnCrystalPickup()
    {
        pickupEffectInstantiator(pickableItems.returnGameObjectForTheKey(_passedOnCollider.tag), _passedOnCollider.transform.position);
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
        foreach (var actionsToBePerformed in _playerActionHandlerDic.Keys)
        {
            if (actionsToBePerformed == collider.tag)
            {
                System.Action action = _playerActionHandlerDic[collider.tag];

                _passedOnCollider = collider;

                action.Invoke(); //invokes the method

                break;

            }
        }
    }
}
