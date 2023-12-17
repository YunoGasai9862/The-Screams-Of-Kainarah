using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static GameObjectCreator;

public class CheckpointColliderListener : MonoBehaviour, IObserver<GameObject>
{
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
                await PlayerSpawn().ResetPlayerAttributes(playerObject, cp.checkpoint.transform.position, new Vector3(0, 2, 0));
            }
        }
    }

    public void OnNotify(GameObject Data, params object[] optional)
    {
      _ = RespawnPlayer(Data, CheckPointsScriptableObjectFetch);
    
    }
}
