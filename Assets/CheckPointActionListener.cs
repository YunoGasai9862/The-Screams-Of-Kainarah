using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CheckPoints;

public class CheckPointActionListener : MonoBehaviour, IObserver<Checkpoint>
{

    private Dictionary<string, Checkpoint> _checkpointsDict = new Dictionary<string, Checkpoint>();

    public Dictionary<string, Checkpoint> CheckpointDict { get => _checkpointsDict; set => _checkpointsDict = value; } 

    private async void Awake()
    {
        CheckpointDict = await PrefillCheckPointsDict(GameObjectCreator.CheckPointsScriptableObjectFetch);
    }

    private async Task<Dictionary<string, Checkpoint>> PrefillCheckPointsDict(CheckPoints scriptableObject)
    {
        var filledDict = new Dictionary<string, Checkpoint>();
        await Task.Run(() =>
        {
            foreach (var value in scriptableObject.checkpoints)
            {
                filledDict.Add(value.checkpoint.tag, value);
            }
        });
        return filledDict;
 
    }
    private void OnEnable()
    {
        PlayerObserverListenerHelper.CheckPointsObserver.AddObserver(this);
    }

    private void OnDisable()
    {
        PlayerObserverListenerHelper.CheckPointsObserver.RemoveOberver(this);
    }
    public void OnNotify(Checkpoint Data, params object[] optional)
    {
        if(CheckpointDict.TryGetValue(Data.checkpoint.tag, out Checkpoint value))
        {

        }
    }
}
