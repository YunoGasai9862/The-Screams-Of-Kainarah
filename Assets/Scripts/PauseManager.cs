using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject PausePanel;
    [SerializeField] GameObject SettingsPanel;
    private OpenClose OC;

   
    public static bool pausedGame = false;
    void Start()
    {
        OC = new OpenClose(PausePanel, SettingsPanel);
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
        SettingsPanel.SetActive(false);
        pausedGame = false;

    }
    public bool isGamePaused()
    {
        return pausedGame;
    }
    public void Settings()
    {
        OC.ToggleSecondPanelOn(); //makes setting panel On
    }
    public void ReturnBackToPause()
    {

        OC.ToggleFirstPanelOn();  //makes First Panel On
    }
    


}
