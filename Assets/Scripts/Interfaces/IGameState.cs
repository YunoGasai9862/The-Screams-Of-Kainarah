
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

public interface IGameState
{
    IEnumerator LoadGame(string saveFileName);

    Task LoadGameAsync(string saveFileName, CancellationToken cancellationToken);

    IEnumerator SaveGame(string fileName);

    Task SaveGameAsync(string fileName, CancellationToken cancellationToken);

    IEnumerator SaveCheckPoint(string saveFileName);

    Task SaveCheckPointAsync(string saveFileName, CancellationToken cancellationToken);

    IEnumerator RestartLevel();

    Task RestartLevelAsync(CancellationToken cancellationToken);

    IEnumerator LoadLastCheckPoint(string saveFileName);

    Task LoadLastCheckPointAsync(string saveFileName, CancellationToken cancellationToken);

    IEnumerator NewGame();

    Task NewGameAsync(CancellationToken cancellationToken);

    IEnumerator LoadScene(int sceneIndex);

    Task LoadSceneAsync(int sceneIndex);
}
