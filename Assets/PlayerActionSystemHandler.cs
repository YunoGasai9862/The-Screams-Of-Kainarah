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

    Dictionary<String, System.Action> _playerActionHandlerDic;

    private IPickable _pickable;

    private GameObjectInstantiator _gameObject;


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
        Debug.Log("Dagger");
    }

    private void OnHealthPickup()
    {
        Debug.Log("Health");
    }

    private void OnCrystalPickup()
    {
        Debug.Log("Crystal");
    }

    private void OnEnable()
    {
        Subject.AddObserver(this); //Add PlayerActionSystem as an observer
    }

    private void OnDisable()
    {
        Subject.RemoveOberver(this); //Remove PlayerActionSystem as an observer when an event is handled/or the observer is no longer needed
    }


    public void managePickable()
    {
        _pickable.AddToInventory(); //adds to the inventory

        _pickable.DestroyPickableFromScene();
    }

    private void pickupEffectInstantiator(GameObject prefab, Vector3 position)
    {
        _gameObject = new(prefab);
        _gameObject.InstantiateGameObject(position, Quaternion.identity);
    }

    public void OnNotify(string itemPickedUp)
    {
        foreach (var actionsToBePerformed in _playerActionHandlerDic.Keys)
        {
            if (actionsToBePerformed == itemPickedUp)
            {
                System.Action action = _playerActionHandlerDic[itemPickedUp];

                action.Invoke(); //invokes the method
            }
        }
    }
}
