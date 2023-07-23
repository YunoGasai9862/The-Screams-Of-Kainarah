using GlobalAccessAndGameHelper;
using PlayerAnimationHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerActionSystemHandler : MonoBehaviour, IObserver
{

    [SerializeField] SubjectsToBeNotified Subject;

    [SerializeField] PickableItemsClass pickableItems;

    Dictionary<String, System.Action> _playerActionHandlerDic;

    private Vector2 _pickableItemPosition;

    private GameObjectInstantiator _gameObject;

    private string pickableItemKey;


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
        //Dagger Throw Logic
    }

    private void OnHealthPickup()
    {
        pickupEffectInstantiator(pickableItems.returnGameObjectForTheKey(pickableItemKey), _pickableItemPosition);
    }

    private void OnCrystalPickup()
    {
        pickupEffectInstantiator(pickableItems.returnGameObjectForTheKey(pickableItemKey), _pickableItemPosition);
        //Add Inventory Logic!!!
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

    public void OnNotify(string itemPickedUp, Vector3 Position)
    {
        foreach (var actionsToBePerformed in _playerActionHandlerDic.Keys)
        {
            if (actionsToBePerformed == itemPickedUp)
            {
                System.Action action = _playerActionHandlerDic[itemPickedUp];

                pickableItemKey = itemPickedUp;

                _pickableItemPosition = Position;

                action.Invoke(); //invokes the method

                break;

            }
        }
    }
}
