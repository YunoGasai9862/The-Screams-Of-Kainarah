
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Target Camera")]
    [SerializeField] Camera _mainCamera;

    [Header("Target Body Animator")]
    [SerializeField] Animator _animator;

    [Header("Shake Min and Max Range")]
    [SerializeField] float _minShake;
    [SerializeField] float _maxShake;

    [Header("Target Body Animation Names For Camera Shake")]
    [SerializeField] List<string> animationNames;

    private PlayerAttackEnum.PlayerAttackSlash currentAttackState;
    private Vector3 _cameraOldPosition;
    private CancellationToken _token;
    private bool _shaking = false;

    private void Start()
    {
        _token = new CancellationToken();
    }

    void Update()
    {
        currentTargetAnimationShake(animationNames);
    }

    private async IAsyncEnumerator<WaitForSeconds> shakeCamera(Camera _mainCamera, float timeForCameraShake) //do it tomorrow
    {
        float timeSpent = 0f;

        _cameraOldPosition = _mainCamera.transform.position;

        while (timeSpent < timeForCameraShake)
        {
            float randomXThrust = Random.Range(_minShake, _maxShake);  //i didn't know this work too, i was using adding to x and y, this will simply shake/translate the camera up and down
            //only and only if there's a camera holder to keep the camera in place
            float randomYThrust = Random.Range(_minShake, _maxShake);

            _mainCamera.transform.position = _cameraOldPosition + new Vector3(randomXThrust, randomYThrust, 0); //to avoid effecting the z-index (also add it to _cameraOldPosition)

            timeSpent += Time.deltaTime;

            await Task.Delay(System.TimeSpan.FromSeconds(.1f));

        }
        _mainCamera.transform.position = _cameraOldPosition; //sets back the position

        yield return new WaitForSeconds(0f); //dummy return value for IAsync Type

    }

    private void currentTargetAnimationShake(List<string> _animationNames)
    {
        foreach(string _animationName in _animationNames)
        {
           if(_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationName) && !_shaking)
            {
                _shaking = true;
                RunAsyncCoroutineWaitForSeconds.RunTheAsyncCoroutine(shakeCamera(_mainCamera, .01f), _token);
                break;
            }

            if(_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationName) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >.7f) //fix this logic
            {
                _shaking = false;
            }
        }

    }

}
