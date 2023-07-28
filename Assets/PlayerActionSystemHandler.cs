using GlobalAccessAndGameHelper;
using PlayerAnimationHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using InventoryManagement = CreateInventorySystem;

public class PlayerActionSystemHandler : MonoBehaviour, IObserver<Collider2D>
{

    [SerializeField] SubjectsToBeNotified Subject;

    [SerializeField] PickableItemsClass pickableItems;

    Dictionary<String, System.Action> _playerActionHandlerDic;

    private GameObjectInstantiator _gameObject;

    private Collider2D _passedOnCollider;

    private PlayerHelperClassForOtherPurposes _playerHelperClassForOtherPurposes;

    private void Awake()
    {
        _playerActionHandlerDic = new Dictionary<String, System.Action>
        {

             { "Crystal", OnCrystalPickup },
             { "Health" , OnHealthPickup  },
             { "Dagger" , OnDaggerPickup  }

        };

        _playerHelperClassForOtherPurposes = (PlayerHelperClassForOtherPurposes)Subject;

        Debug.Log(_playerHelperClassForOtherPurposes);

    }

    private void OnDaggerPickup()
    {
        GameObject temp = pickableItems.returnGameObjectForTheKey(_passedOnCollider.tag);

        InventoryManagement.AddToInventory(temp.GetComponent<SpriteRenderer>().sprite, temp.tag); //adds it to the inventory

    }

    private void OnHealthPickup()
    {
        pickupEffectInstantiator(pickableItems.returnGameObjectForTheKey(_passedOnCollider.tag), _passedOnCollider.transform.position);

    }

    private void OnCrystalPickup()
    {
        pickupEffectInstantiator(pickableItems.returnGameObjectForTheKey(_passedOnCollider.tag), _passedOnCollider.transform.position);
    }

    private void OnEnable()
    {
        Subject.AddObserver(this); //Add PlayerActionSystem as an observer
    }

    private void OnDisable()
    {
        Subject.RemoveOberver(this); //Remove PlayerActionSystem as an observer when an event is handled/or the observer is no longer needed
    }

    private void pickupEffectInstantiator(GameObject prefab, Vector3 position)
    {
        _gameObject = new(prefab);
        _gameObject.InstantiateGameObject(position, Quaternion.identity);
    }

    public void OnNotify(ref Collider2D collider)
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
