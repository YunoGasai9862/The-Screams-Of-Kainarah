
using Assets.Scripts.Scene;

public class KeepTrackOfLightning : MonoBehaviorScene
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
