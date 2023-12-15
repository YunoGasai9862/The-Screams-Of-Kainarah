using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CheckPoints;
using static GameObjectCreator;

public class CheckPointActionListener : MonoBehaviour, IObserver<Checkpoint>, IObserver<GameObject>
{

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
                checkPointsScriptableObjectFetch.checkpoints[i] = await SetAsCurrentRespawnCheckPoint(value, false); 
        }

        await Task.Delay(TimeSpan.FromSeconds(0));
    }

    private Task<Checkpoint> SetAsCurrentRespawnCheckPoint(Checkpoint value, bool shouldRespawn)
    {
       Checkpoint newValue = new Checkpoint
       {
           checkpoint= value.checkpoint,
           finishLevelCheckpoint= value.finishLevelCheckpoint,
           shouldResetPlayerAttributes= value.shouldResetPlayerAttributes,
           shouldRespawn = shouldRespawn
       };

        return Task.FromResult(newValue);
    }
    private async Task RespawnPlayer(GameObject playerObject, CheckPoints checkPointsScriptableObjectFetch)
    {
        foreach(var cp in checkPointsScriptableObjectFetch.checkpoints)
        {
            if (cp.shouldRespawn)
                await PlayerSpawn().spawnPlayer(playerObject, cp.checkpoint.transform.position);
                
        }
    }

    private bool IsPlayerDead(AbstractEntity playerData)
    {
       return playerData.Health == 0 ? true : false;
    }

    private void OnEnable()
    {
        PlayerObserverListenerHelper.CheckPointsObserver.AddObserver(this);
        PlayerObserverListenerHelper.MainPlayerListener.AddObserver(this);
    }

    private void OnDisable()
    {
        PlayerObserverListenerHelper.CheckPointsObserver.RemoveOberver(this);
        PlayerObserverListenerHelper.MainPlayerListener.RemoveOberver(this);
    }
    public void OnNotify(Checkpoint Data, params object[] optional)
    {
        if(CheckpointDict.TryGetValue(Data.checkpoint.tag, out Func<Checkpoint, CheckPoints, Task > value))
        {
          value.Invoke(Data, CheckPointsScriptableObjectFetch); //invokes that particular function to reset checkpoints
        }
    }

    public void OnNotify(GameObject Data, params object[] optional)
    {
        if(Data.GetComponent<AbstractEntity>() != null)
        {
            AbstractEntity playerData = Data.GetComponent<AbstractEntity>();
            Debug.Log(playerData);
            if (IsPlayerDead(playerData))
              _=  RespawnPlayer(Data, CheckPointsScriptableObjectFetch);
        }
    }
}
