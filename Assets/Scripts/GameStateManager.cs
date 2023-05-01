using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameStateManager : MonoBehaviour
{
  

    public static void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void RestartGame(Rigidbody2D rb)
    {
        rb.bodyType = RigidbodyType2D.Static;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void ChangeLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex + 1);
    }




}
