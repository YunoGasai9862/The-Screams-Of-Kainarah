using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IGameState
{
    Task LoadGame();
    Task SaveGame();
    Task  RestartLevel();
    Task LoadLastCheckPoint();

}
