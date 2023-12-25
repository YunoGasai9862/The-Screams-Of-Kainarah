using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SceneData;

public class PlayerCurrentState : MonoBehaviour, IGameStateHandler
{
    private void Start()
    {
        GameObjectCreator.InsertIntoGameStateHandlerList(this);
    }
    public void GameStateHandler(SceneData data)
    {
        ObjectData playerData = new ObjectData(transform.tag, transform.name,transform.position, transform.rotation);
        Debug.Log(playerData.ToString());
        data.AddToObjectsToPersist(playerData);
    }
}
