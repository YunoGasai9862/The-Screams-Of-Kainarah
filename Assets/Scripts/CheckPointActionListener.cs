using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CheckPoints;
using static GameObjectCreator;

public class CheckPointActionListener : MonoBehaviour, IObserver<Checkpoint>
{
    [SerializeField]
    public string saveFileName;

    private Dictionary<string, Func<Checkpoint, CheckPoints, Task>> _checkpointsDict = new Dictionary<string, Func<Checkpoint, CheckPoints, Task>>();

    public Dictionary<string, Func<Checkpoint, CheckPoints, Task>> CheckpointDict { get => _checkpointsDict; set => _checkpointsDict = value; } 

    private async void Awake()
    {
        CheckpointDict = await PrefillCheckPointsDict(CheckPointsScriptableObjectFetch);
    }

    private Task<Dictionary<string, Func<Checkpoint, CheckPoints, Task>>> PrefillCheckPointsDict(CheckPoints checkPointsScriptableObjectFetch)
    {
        var filledDict = new Dictionary<string, Func<Checkpoint, CheckPoints, Task>>();

        foreach (var value in checkPointsScriptableObjectFetch.checkpoints)
        {
            filledDict.Add(value.checkpoint.tag, (value, scriptableObject) => PerformCheckPointOperation(value, scriptableObject));
        }

        return Task.FromResult(filledDict);
    }

    private async Task PerformCheckPointOperation(Checkpoint value, CheckPoints checkPointsScriptableObjectFetch)
    {
        //overwrite the values with the values sent in by the player
        //remove previous respawn checkpoint bools, and add the bool to the current one
        for(int i=0;  i< checkPointsScriptableObjectFetch.checkpoints.Length; i++)
        {
            if(value.checkpoint.tag == checkPointsScriptableObjectFetch.checkpoints[i].checkpoint.tag)
            {
                checkPointsScriptableObjectFetch.checkpoints[i] = await SetAsCurrentRespawnCheckPoint(value, true); //update the value
            }else
                checkPointsScriptableObjectFetch.checkpoints[i] = await SetAsCurrentRespawnCheckPoint(checkPointsScriptableObjectFetch.checkpoints[i], false); 
        }

        await Task.Delay(TimeSpan.FromSeconds(0));
    }

    private Task<Checkpoint> SetAsCurrentRespawnCheckPoint(Checkpoint value, bool shouldRespawn)
    {
        Checkpoint newValue = new Checkpoint
        {
           checkpoint = value.checkpoint,
           finishLevelCheckpoint= value.finishLevelCheckpoint,
           shouldResetPlayerAttributes= value.shouldResetPlayerAttributes,
           shouldRespawn = shouldRespawn
       };

        return Task.FromResult(newValue);
    }

    private void OnEnable()
    {
        PlayerObserverListenerHelper.CheckPointsObserver.AddObserver(this);
    }

    private void OnDisable()
    {
        PlayerObserverListenerHelper.CheckPointsObserver.RemoveOberver(this);

        //reset checkpoints
        ResetCheckpoints(CheckPointsScriptableObjectFetch, false, false, false);
    }

    private void ResetCheckpoints(CheckPoints checkPointsScriptableObjectFetch, bool finishLevelBool, bool shouldRespawnBool, bool shouldResetAttributesBool)
    {
        foreach (var cp in checkPointsScriptableObjectFetch.checkpoints)
        {
            cp.checkpoint.SetActive(true);
            cp.finishLevelCheckpoint = finishLevelBool;
            cp.shouldRespawn= shouldRespawnBool;
            cp.shouldResetPlayerAttributes = shouldResetAttributesBool;
        }

    }
    public void OnNotify(Checkpoint Data, params object[] optional)
    {
        if(CheckpointDict.TryGetValue(Data.checkpoint.tag, out Func<Checkpoint, CheckPoints, Task > value))
        {
            value.Invoke(Data, CheckPointsScriptableObjectFetch); //invokes that particular function to reset checkpoints 
            //call the checkpoint => Save Game method
            Debug.Log("On Notify Executing!!");
            _ = GameStateManager.instance.SaveCheckPoint(saveFileName);
        }
    }


}
