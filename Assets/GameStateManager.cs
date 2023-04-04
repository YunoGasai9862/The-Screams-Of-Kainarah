using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

    public enum GameState{

        BEGAN, PLAYING,EXIT
    }


    public static bool GAMEBEGAN;
    private float timepassedforpreviousstate = 0f;
    void Start()
    {
        GAMEBEGAN = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
