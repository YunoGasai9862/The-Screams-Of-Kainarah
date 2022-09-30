
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool isOver = false;
    void GameOver()
    {
        while(!isOver)
        {
            Restart();
            isOver = true;
        }
    }
   void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
