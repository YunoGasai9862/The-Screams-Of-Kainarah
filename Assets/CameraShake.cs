
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
    void Start()
    {
        _cameraOldPosition= _mainCamera.transform.position; 
    }


    void Update()
    {
        if(_canShake)
        {
            _mainCamera.transform.position +=new Vector3(_mainCamera.transform.position.x + 2f, _mainCamera.transform.position.y + 2f, _mainCamera.transform.position.z);

        }
        while (_timeSpentShaking < 10f)
        {
            _timeSpentShaking += Time.deltaTime;
            _canShake = true;
        }

        if(_timeSpentShaking > 10f)
        {
            _canShake = false;
            _mainCamera.transform.position = _cameraOldPosition;
        }

    }

    private async Task<bool> shakeCamera(Camera _camera, float strength, float shiftX, float shiftY) //do it tomorrow
    {
        await Task.Delay(100);
        return  false;
    }
}
