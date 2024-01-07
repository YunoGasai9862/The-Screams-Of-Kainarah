
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Target Camera")]
    [SerializeField] Camera _mainCamera;

    [Header("Target Body Tag")]
    [SerializeField] string targetBodyTag;

    [Header("Shake Min and Max Range")]
    [SerializeField] float _minShake;
    [SerializeField] float _maxShake;

    [Header("Target Body Animation Names For Camera Shake")]
    [SerializeField] List<string> animationNames;

    private Vector3 _cameraOldPosition;
    private CancellationToken _token;
    private CancellationTokenSource _cancellationTokenSource;
    private bool _shaking = false;
    private GameObject _targetBody;
    private Animator _animator;

    public bool isShaking { get => _shaking; set => _shaking = value; }

    private void Start()
    {
        _cancellationTokenSource= new CancellationTokenSource();
        _token = _cancellationTokenSource.Token;
    }
    void Update()
    {  
        if(_targetBody == null)
        {
            _targetBody = GameObject.FindWithTag(targetBodyTag);
            _animator = _targetBody.GetComponent<Animator>();
        }
        else
            currentTargetAnimationShake(animationNames);

    }

    private async IAsyncEnumerator<WaitForSeconds> shakeCamera(Camera _mainCamera, float timeForCameraShake) //do it tomorrow
    {
        float timeSpent = 0f;

        _cameraOldPosition = _mainCamera.transform.position;

        while (timeSpent < timeForCameraShake)
        {
            float randomXThrust = UnityEngine.Random.Range(_minShake, _maxShake);  //i didn't know this work too, i was using adding to x and y, this will simply shake/translate the camera up and down
            //only and only if there's a camera holder to keep the camera in place
            float randomYThrust = UnityEngine.Random.Range(_minShake, _maxShake);

            _mainCamera.transform.position = _cameraOldPosition + new Vector3(randomXThrust, randomYThrust, 0); //to avoid effecting the z-index (also add it to _cameraOldPosition)

            timeSpent += Time.deltaTime;

            await Task.Delay(TimeSpan.FromSeconds(.05f));

        }
        _mainCamera.transform.position = _cameraOldPosition; //sets back the position

        _shaking = false;

        yield return new WaitForSeconds(0f); //dummy return value for IAsync Type

    }

    private void currentTargetAnimationShake(List<string> _animationNames)
    {
        foreach(string _animationName in _animationNames)
        {
           if(_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationName) && !isShaking)
            {
                isShaking = true;

                RunAsyncCoroutineWaitForSeconds.RunTheAsyncCoroutine(shakeCamera(_mainCamera, .03f), _token);

                break;
            }

        }

    }

}
