using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCurrentState : MonoBehaviour, IGameStateHandler
{
    private void Awake()
    {
        GameObjectCreator.InsertIntoGameStateHandlerList(GetComponent<IGameStateHandler>());
    }
    public void GameStateHandler(SceneData data)
    {
       
    }
}
