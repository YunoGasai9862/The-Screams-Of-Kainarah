using Assets.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static CheckPoints;

[Observer(ObserverType = typeof(CheckPointActionListener), SubjectType = typeof(EntityPoolManager), ContextType = typeof({))]
public class CheckPointActionListener : MonoBehaviour, IObserver<CheckPoints.Checkpoint>, INotify<EntityPoolManager>
{
    [SerializeField]
    public string saveFileName;

    private CheckPoints CheckpointsSO { get; set; }

    private Delegator Delegator { get; set; }

    private Dictionary<string, Func<CheckPoints.Checkpoint, CheckPoints, Task>> _checkpointsDict = new Dictionary<string, Func<CheckPoints.Checkpoint, CheckPoints, Task>>();

    public Dictionary<string, Func<CheckPoints.Checkpoint, CheckPoints, Task>> CheckpointDict { get => _checkpointsDict; set => _checkpointsDict = value; }

    private async void Awake()
    {
        Delegator = await Helper.GetDelegator<Delegator>();

        Delegator.NotifySubjectWrapper(new ObserverContext<CheckPoints>()
        {
            Instance = gameObject,
            SubjectType = typeof(EntityPoolManager)

        }, this);
    }

    private Dictionary<string, Func<CheckPoints.Checkpoint, CheckPoints, Task>> PrefillCheckPointsDict(CheckPoints checkPointsScriptableObjectFetch)
    {
        var filledDict = new Dictionary<string, Func<CheckPoints.Checkpoint, CheckPoints, Task>>();

        foreach (var value in checkPointsScriptableObjectFetch.checkpoints)
        {
            filledDict.Add(value.checkpoint.tag, (value, scriptableObject) => PerformCheckPointOperation(value, scriptableObject));
        }

        return filledDict;
    }

    private async Task PerformCheckPointOperation(CheckPoints.Checkpoint value, CheckPoints checkPointsScriptableObjectFetch)
    {
        //overwrite the values with the values sent in by the player
        //remove previous respawn checkpoint bools, and add the bool to the current one
        for (int i = 0; i < checkPointsScriptableObjectFetch.checkpoints.Length; i++)
        {
            if (value.checkpoint.tag == checkPointsScriptableObjectFetch.checkpoints[i].checkpoint.tag)
            {
                checkPointsScriptableObjectFetch.checkpoints[i] = await SetAsCurrentRespawnCheckPoint(value, true); //update the value
            }
            else
                checkPointsScriptableObjectFetch.checkpoints[i] = await SetAsCurrentRespawnCheckPoint(checkPointsScriptableObjectFetch.checkpoints[i], false);
        }

        await Task.Delay(TimeSpan.FromSeconds(0));
    }

    private Task<Checkpoint> SetAsCurrentRespawnCheckPoint(CheckPoints.Checkpoint value, bool shouldRespawn)
    {
        CheckPoints.Checkpoint newValue = new CheckPoints.Checkpoint
        {
            checkpoint = value.checkpoint,
            finishLevelCheckpoint = value.finishLevelCheckpoint,
            shouldResetPlayerAttributes = value.shouldResetPlayerAttributes,
            shouldRespawn = shouldRespawn
        };

        return Task.FromResult(newValue);
    }

    private void OnDisable()
    {
        ResetCheckpoints(CheckpointsSO, false, false, false);
    }

    private void ResetCheckpoints(CheckPoints checkPointsScriptableObjectFetch, bool finishLevelBool, bool shouldRespawnBool, bool shouldResetAttributesBool)
    {
        foreach (var cp in checkPointsScriptableObjectFetch.checkpoints)
        {
            cp.checkpoint.SetActive(true);
            cp.finishLevelCheckpoint = finishLevelBool;
            cp.shouldRespawn = shouldRespawnBool;
            cp.shouldResetPlayerAttributes = shouldResetAttributesBool;
        }

    }

    public void OnNotify(CheckPoints.Checkpoint data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        if (CheckpointDict.TryGetValue(data.checkpoint.tag, out Func<Checkpoint, CheckPoints, Task> value))
        {
            value.Invoke(data, SceneSingleton.CheckPoints); //invokes that particular function to reset checkpoints 

            GameStateManager.instance.SaveCheckPoint(saveFileName);
        }

        yield return null;
    }

    public IEnumerator Notify(CheckPoints value)
    {
        CheckpointsSO = value;

        PrefillCheckPointsDict(CheckpointsSO);

        yield return null;
    }
}
