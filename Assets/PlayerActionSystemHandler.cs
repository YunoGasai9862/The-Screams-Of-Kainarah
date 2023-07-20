using PlayerAnimationHandler;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerActionSystemHandler : MonoBehaviour, IObserver
{
    [SerializeField] SubjectsToBeNotified Subject;

    Dictionary<AnimationStateKeeper.StateKeeper, System.Action> _playerActionHandlerDic;

    private void Awake()
    {
        _playerActionHandlerDic = new Dictionary<AnimationStateKeeper.StateKeeper, System.Action>
        {

            //adds the values
        };
    }


    private void OnEnable()
    {
        Subject.AddObserver(this); //Add PlayerActionSystem as an observer
    }

    private void OnDisable() 
    {
        Subject.RemoveOberver(this); //Remove PlayerActionSystem as an observer when an event is handled/or the observer is no longer needed
    }

    public void OnNotify(AnimationStateKeeper.StateKeeper stateNotifier)
    {
       
    }
}
