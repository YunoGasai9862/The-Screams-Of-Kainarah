
using PlayerAnimationHandler;
using System.Threading.Tasks;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;
    private int _currentPlayerState = AnimationStateKeeper.currentPlayerState; //to see if the player is attacking or not
    private Vector3 _cameraOldPosition;
    private bool _canShake = true;
    private float _timeSpentShaking = 0f;
    private float modifier=1;
    void Start()
    {
        _cameraOldPosition= _mainCamera.transform.position; 
    }


    void Update()
    {
       
    }

    private async Task<bool> shakeCamera(Camera _camera, float strength,  float timeForCameraShake) //do it tomorrow
    {
        float timeSpent = 0f;

        while(timeSpent < timeForCameraShake)
        {
            float randomXThrust = Random.Range(-1f, 1f);  //i didn't know this work too, i was using adding to x and y, this will simply shake/translate the camera up and down
            //only and only if there's a camera holder to keep the camera in place
            float randomYThrust = Random.Range(-1f, 1f);    

            _camera.transform.position = new Vector3(randomYThrust, randomXThrust, _camera.transform.position.z);
            await Task.Delay(100);
            timeSpent += randomXThrust;

        }
        return  false;
    }
}
