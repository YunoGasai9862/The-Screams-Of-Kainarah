
using System.Threading;
using System.Threading.Tasks;

public interface IGameState
{
    Task LoadGame(string saveFileName, SemaphoreSlim lockingThread);
    Task SaveGame(string fileName); //for manipulating
    Task SaveCheckPoint(string saveFileName);
    Task  RestartLevel();
    Task LoadLastCheckPoint(string saveFileName, SemaphoreSlim lockingThread);
    Task NewGame();
    Task LoadSceneAsync(int sceneIndex);

}
