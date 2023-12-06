using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameStateManager : MonoBehaviour
{
    public async static Task RestartGame()
    {
        await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public static void ChangeLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex + 1);
    }

}
