using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IGameState
{
    Task LoadGame();
    Task SaveGame(SceneData sceneData); //for manipulating
    Task SaveCheckPoint(string saveFileName);
    Task  RestartLevel();
    Task LoadLastCheckPoint(string saveFileName);
    Task NewGame();

}
