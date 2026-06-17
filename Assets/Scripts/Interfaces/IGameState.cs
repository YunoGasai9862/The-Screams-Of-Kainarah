
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

public interface IGameState
{
    IEnumerator LoadGame(string saveFileName);
    IEnumerator SaveGame(string fileName);
    IEnumerator SaveCheckPoint(string saveFileName);
    IEnumerator RestartLevel();
    IEnumerator LoadLastCheckPoint(string saveFileName);
    IEnumerator NewGame();
    IEnumerator LoadSceneAsync(int sceneIndex);

}
