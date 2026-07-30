using Assets.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using Annotations.Enums;
using Assets.Scripts.Scene;
using UnityEngine.SceneManagement;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(CheckPointActionListener), SubjectType = typeof(EntityPoolManager), ContextType = typeof(EntityPoolManager))]
[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(CheckPointActionListener), SubjectType = typeof(GameStateManager), ContextType = typeof(GameStateManager))]
public class CheckPointActionListener : Assets.Scripts.Scene.Scene, INotify<EntityPoolManager>, INotify<GameStateManager>
{
    private static string CHECKPOINTS_KEY = "CheckPoints";  

    [SerializeField]
    public string saveFileName;

    private CheckPoints CheckpointsSO { get; set; }

    private EntityPoolManager EntityPoolManagerInstance { get; set; }

    private GameStateManager GameStateManagerInstance { get; set; }

    private Delegator Delegator { get; set; }

    private Dictionary<string, Func<CheckPoints.Checkpoint, CheckPoints, Task>> _checkpointsDict = new Dictionary<string, Func<CheckPoints.Checkpoint, CheckPoints, Task>>();

    public Dictionary<string, Func<CheckPoints.Checkpoint, CheckPoints, Task>> CheckpointDict { get => _checkpointsDict; set => _checkpointsDict = value; }

    private SceneUtils SceneUtils { get; set; }

    private async void Start()
    {
        SceneUtils = await BaseScene.GetSceneUtilsAsync();

        StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));

        Delegator.NotifySubjectWrapper(new ObserverContext<GameStateManager>()
        {
            Instance = gameObject,
            EntityType = typeof(CheckPointActionListener),
            SubjectType = typeof(GameStateManager)

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

    private Task<CheckPoints.Checkpoint> SetAsCurrentRespawnCheckPoint(CheckPoints.Checkpoint value, bool shouldRespawn)
    {
        CheckPoints.Checkpoint newValue = new CheckPoints.Checkpoint(value.guid, value.checkpoint, value.shouldResetPlayerAttributes, value.shouldRespawn, value.finishLevelCheckpoint);

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

    public IEnumerator Notify(EntityPoolManager value)
    {
        EntityPoolManagerInstance = value;

        CheckpointsSO = SceneUtils.GetFromEntityPoolManager<CheckPoints>(EntityPoolManagerInstance, CHECKPOINTS_KEY);
            
        PrefillCheckPointsDict(CheckpointsSO);

        yield return null;
    }

    public IEnumerator Notify(CheckPoints.Checkpoint value)
    {
        if (CheckpointDict.TryGetValue(value.checkpoint.tag, out Func<CheckPoints.Checkpoint, CheckPoints, Task> val))
        {
            val.Invoke(value, CheckpointsSO); //invokes that particular function to reset checkpoints 

            yield return new WaitUntil(() => GameStateManagerInstance != null);

            GameStateManagerInstance.SaveCheckPoint(value.guid, "1.0");
        }

        yield return null;
    }

    public IEnumerator Notify(GameStateManager value)
    {
        GameStateManagerInstance = value;

        yield return null;
    }
}
