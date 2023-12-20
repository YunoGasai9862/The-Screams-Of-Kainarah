using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IGameState
{
    Task LoadGame();
    Task SaveGame(SceneData sceneData); //for manipulating
    Task SaveCheckPoint(SceneData sceneData);
    Task  RestartLevel();
    Task LoadLastCheckPoint();
    Task NewGame();

}
