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

    private async Task RespawnPlayer(GameObject playerObject, CheckPoints checkPointsScriptableObjectFetch)
    {
        foreach (var cp in checkPointsScriptableObjectFetch.checkpoints)
        {
            if (cp.shouldRespawn)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;
                await PlayerSpawn().ResetAnimationAndMaterialProperties(playerObject, _cancellationToken);
                await GameStateManager.instance.LoadLastCheckPoint(GameStateManager.instance.GetFileLocationToLoad);
            }
        }
    }

    public void OnNotify(GameObject Data, params object[] optional)
    {
      _ = RespawnPlayer(Data, CheckPointsScriptableObjectFetch);
    
    }
}
