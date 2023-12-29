using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static GameObjectCreator;

public class CheckpointColliderListener : MonoBehaviour, IObserver<GameObject>
{
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;

    private void OnEnable()
    {
        PlayerObserverListenerHelper.MainPlayerListener.AddObserver(this);
    }

    private void OnDisable()
    {
        PlayerObserverListenerHelper.MainPlayerListener.RemoveOberver(this);
    }

    private async Task RespawnPlayer(GameObject playerObject, CheckPoints checkPointsScriptableObjectFetch, SemaphoreSlim lockingThread)
    {
        foreach (var cp in checkPointsScriptableObjectFetch.checkpoints)
        {
            if (cp.shouldRespawn)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;
                await PlayerSpawn().ResetAnimationAndMaterialProperties(playerObject, _cancellationToken);
                await GameStateManager.instance.LoadLastCheckPoint(GameStateManager.instance.GetFileLocationToLoad, lockingThread); //make sure it happens only once
            }
        }
    }

    public void OnNotify(GameObject Data, params object[] optional) //passing it here, maybe think of a better approach later?
    {
        SemaphoreSlim lockingThread = optional[0] as SemaphoreSlim;
        _ = RespawnPlayer(Data, CheckPointsScriptableObjectFetch, lockingThread);
    }
}
