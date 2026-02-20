using Assets.Annotations;
using System.Collections;
using UnityEngine;

[Observer(ObserverType = typeof(CheckpointColliderListener), SubjectType = typeof(EntityPoolManager), ContextType = typeof(EntityPoolManager))]
[Observer(SubjectType = typeof(PlayerActionRelayer), ObserverType = typeof(CheckpointColliderListener), ContextType = typeof(GameObject))]
public class CheckpointColliderListener : MonoBehaviour, INotify<GameObject>, INotify<EntityPoolManager>
{
    private static string CHECKPOINTS_KEY = "CheckPoints";
    private EntityPoolManager EntityPoolManagerInstance { get; set; }

    private CheckPoints CheckPointsSO { get; set; }

    private IEnumerator RespawnPlayer(GameObject playerObject, CheckPoints checkPointsScriptableObjectFetch)
    {
        foreach (var cp in checkPointsScriptableObjectFetch.checkpoints)
        {
            if (cp.shouldRespawn)
            {
                //TODO for the reset animation/Material
                await SceneSingleton.PlayerSpawn().ResetAnimationAndMaterialProperties(playerObject, _cancellationToken);
                GameStateManager.instance.LoadLastCheckPoint(GameStateManager.instance.GetFileLocationToLoad, lockingThread); //make sure it happens only once
            }
        }

        yield return null;
    }

    public IEnumerator Notify(GameObject value)
    {
        if (CheckPointsSO == null || EntityPoolManagerInstance == null)
        {
            Debug.LogError("Either the CheckPointsSO is null or the EntityPoolManagerInstance  - please get this rectified/looked into!");
            yield return null;
        }

        yield return RespawnPlayer(value, CheckPointsSO);
    }

    public IEnumerator Notify(EntityPoolManager value)
    {
        EntityPoolManagerInstance = value;

        CheckPointsSO = Helper.GetFromEntityPoolManager<CheckPoints>(EntityPoolManagerInstance, CHECKPOINTS_KEY);

        yield return null;
    }
}
