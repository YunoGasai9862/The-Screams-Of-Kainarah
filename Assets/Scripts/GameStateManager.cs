using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameStateManager : MonoBehaviour, IGameState
{
    public static void ChangeLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex + 1);
    }

    public async Task LoadGame() //implement LoadGame with Json etc by saving states
    {
        await Task.CompletedTask;
    }
    public async Task LoadLastCheckPoint()
    {
        await Task.CompletedTask;

    }
    public async Task SaveGame()
    {
        await Task.CompletedTask;
    }

    public async Task RestartLevel()
    {
        await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
}
