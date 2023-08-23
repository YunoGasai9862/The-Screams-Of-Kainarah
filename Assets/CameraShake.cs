
using PlayerAnimationHandler;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;
    private int _currentPlayerState = AnimationStateKeeper.currentPlayerState; //to see if the player is attacking or not
    private Vector3 _cameraOldPosition;
    private bool _canShake = true;
    private CancellationToken _token;

    private void Start()
    {
        _token = new CancellationToken();
    }

    void Update()
    {
        if (_canShake)
        {
            _canShake = false;
            RunAsyncCoroutine.RunTheAsyncCoroutine(shakeCamera(_mainCamera, .1f), _token);
        }

        if(Input.GetKey(KeyCode.H))
        {
            _canShake = true;
        }
    }

    private async IAsyncEnumerator<bool> shakeCamera(Camera _mainCamera, float timeForCameraShake) //do it tomorrow
    {
        float timeSpent = 0f;

        _cameraOldPosition = _mainCamera.transform.position;

        while(timeSpent < timeForCameraShake)
        {
            float randomXThrust = Random.Range(-1f, 1f);  //i didn't know this work too, i was using adding to x and y, this will simply shake/translate the camera up and down
            //only and only if there's a camera holder to keep the camera in place
            float randomYThrust = Random.Range(-1f, 1f);

            _mainCamera.transform.position = new Vector3(randomXThrust, randomYThrust , _mainCamera.transform.position.z);

            timeSpent += Time.deltaTime;

            await Task.Delay(100);

        }

        _mainCamera.transform.position = _cameraOldPosition; //sets back the position

        yield return false;

    }
}
