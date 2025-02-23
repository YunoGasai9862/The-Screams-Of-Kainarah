using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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
                await SceneSingleton.PlayerSpawn().ResetAnimationAndMaterialProperties(playerObject, _cancellationToken);
                await GameStateManager.instance.LoadLastCheckPoint(GameStateManager.instance.GetFileLocationToLoad, lockingThread); //make sure it happens only once
            }
        }
    }

    public async void OnNotify(GameObject data, NotificationContext notificationContext, params object[] optional)
    {
        SemaphoreSlim lockingThread = optional[0] as SemaphoreSlim;

        await RespawnPlayer(data, SceneSingleton.CheckPoints, lockingThread);
    }
}
