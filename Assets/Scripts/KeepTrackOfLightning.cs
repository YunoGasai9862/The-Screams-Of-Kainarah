using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepTrackOfLightning : MonoBehaviour
{
   
    void Update()
    {
        if(PauseManager.pausedGame)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }else
        {
            transform.GetChild(0).gameObject.SetActive(false);

        }
    }
}
