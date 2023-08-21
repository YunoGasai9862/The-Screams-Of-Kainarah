
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
        if(_canShake)
        {
            _mainCamera.transform.position =new Vector3(_mainCamera.transform.position.x +  _timeSpentShaking, _mainCamera.transform.position.y +  _timeSpentShaking, -10f);


        }
        if (_timeSpentShaking < 2f && modifier == 1 || (_timeSpentShaking > 0f && modifier == -1))
        {

            _timeSpentShaking += (modifier) * Time.deltaTime;
            _canShake = true;
        }

        if(_timeSpentShaking > 2f  && modifier == 1 || (_timeSpentShaking  < 0f && modifier == -1))
        {
            _canShake = false;
           // _mainCamera.transform.position = _cameraOldPosition;
        }

        if(!_canShake)
        {
            _canShake = true;
            modifier *= -1;
            
         }

    }

    private async Task<bool> shakeCamera(Camera _camera, float strength, float shiftX, float shiftY) //do it tomorrow
    {
        await Task.Delay(100);
        return  false;
    }
}
