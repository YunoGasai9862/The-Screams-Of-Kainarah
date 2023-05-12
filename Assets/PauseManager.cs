using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject PausePanel;
    private bool pausedGame = false;
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pausedGame=!pausedGame;
            PauseGame();
        }


    }
    public void PauseGame()
    {
        if(pausedGame)
        {
            Time.timeScale = 0f;
            BringUpMenu();

        }
        else
        {
            ResumeGame();
        }
       
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        closeMenu();

    }
    public void BringUpMenu()
    {
        PausePanel.SetActive(true);

    }
    public void closeMenu()
    {
        PausePanel.SetActive(false);

    }
    public bool isGamePaused()
    {
        return pausedGame;
    }

}
