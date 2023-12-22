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
        ObjectData playerData = new ObjectData(transform.position, transform.rotation, transform.tag, transform.name);
        playerData.AddToObjectsToPersist(playerData);   
    }
}
