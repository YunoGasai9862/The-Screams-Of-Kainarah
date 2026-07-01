
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

public interface IGameState
{
    IEnumerator LoadGame(System.Guid id);

    Task LoadGameAsync(System.Guid id, CancellationToken cancellationToken);

    IEnumerator SaveGame(System.Guid id, string sceneVersion);

    Task SaveGameAsync(System.Guid id, string sceneVersion, CancellationToken cancellationToken);

    IEnumerator SaveCheckPoint(System.Guid id, string sceneVersion);

    Task SaveCheckPointAsync(System.Guid id, string sceneVersion, CancellationToken cancellationToken);

    IEnumerator RestartLevel();

    Task RestartLevelAsync(CancellationToken cancellationToken);

    IEnumerator LoadLastCheckPoint(System.Guid id);

    Task LoadLastCheckPointAsync(System.Guid id, CancellationToken cancellationToken);

    IEnumerator NewGame();

    Task NewGameAsync(CancellationToken cancellationToken);

    IEnumerator LoadScene(int sceneIndex);

    Task LoadSceneAsync(int sceneIndex);
}
