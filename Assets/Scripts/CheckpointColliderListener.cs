using Assets.Annotations;
using System.Collections;
using System.Threading;
using UnityEngine;

[Observer(SubjectType = typeof(PlayerActionRelayer), ObserverType = typeof(CheckpointColliderListener), ContextType = typeof(GameObject))]
public class CheckpointColliderListener : MonoBehaviour, INotify<GameObject>
{
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
        yield return RespawnPlayer(value, SceneSingleton.CheckPoints);
    }
}
