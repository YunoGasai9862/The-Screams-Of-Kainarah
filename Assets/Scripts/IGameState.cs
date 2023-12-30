using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public interface IGameState
{
    Task LoadGame(string saveFileName, Semaphore lockingThread);
    Task SaveGame(string fileName); //for manipulating
    Task SaveCheckPoint(string saveFileName);
    Task  RestartLevel();
    Task LoadLastCheckPoint(string saveFileName, SemaphoreSlim lockingThread);
    Task NewGame();

}
