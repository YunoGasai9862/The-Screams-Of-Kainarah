
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CameraShake : MonoBehaviour, IObserver<AsyncCoroutine>, IObserver<GenericStateBundle<EmitAnimationStateBundle>>
{
    [Header("Target Camera")]
    [SerializeField] Camera _mainCamera;

    [Header("Shake Min and Max Range")]
    [SerializeField] float _minShake;
    [SerializeField] float _maxShake;

    [Header("Time for shake")]
    [SerializeField] float timeForShake; //0.03f (old)

    [Header("Delay Between Each Shake")]
    [SerializeField] float delay; //0.05f (old)

    [Header("Async Coroutine Delegator")]
    [SerializeField] AsyncCoroutineDelegator asyncCoroutineDelegator;

    [Header("Emit Animation Delegator")]
    [SerializeField] EmitAnimationStateDelegator emitAnimationStateDelegator;

    private Vector3 _cameraOldPosition;

    private CancellationToken _token;

    private CancellationTokenSource _cancellationTokenSource;

    private AsyncCoroutine AsyncCoroutine { get; set; }

    private GenericStateBundle<EmitAnimationStateBundle> EmitAnimationStateBundle { get; set; } = new GenericStateBundle<EmitAnimationStateBundle>();

    private void Start()
    {
        _cancellationTokenSource= new CancellationTokenSource();

        _token = _cancellationTokenSource.Token;

        StartCoroutine(asyncCoroutineDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(AsyncCoroutine).ToString()), CancellationToken.None));

        StartCoroutine(emitAnimationStateDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(EmitAnimationStateConsumer).ToString()), CancellationToken.None));
    }

    private async IAsyncEnumerator<WaitForSeconds> shakeCamera(Camera _mainCamera, float timeForCameraShake)
    {
        float timeSpent = 0f;

        _cameraOldPosition = _mainCamera.transform.position;

        while (timeSpent < timeForCameraShake)
        {
            _mainCamera.transform.position = _cameraOldPosition + new Vector3(UnityEngine.Random.Range(_minShake, _maxShake), UnityEngine.Random.Range(_minShake, _maxShake), 0);

            timeSpent += Time.deltaTime;

            await Task.Delay(TimeSpan.FromSeconds(delay));
        }

        _mainCamera.transform.position = _cameraOldPosition; 

        yield return new WaitForSeconds(0f);
    }

    private IEnumerator ExecuteShakeAnimation(EmitAnimationStateBundle stateBundle)
    {
        if (!stateBundle.IsRunning)
        {
            yield return null;
        }

        yield return new WaitUntil(() => AsyncCoroutine != null);
        
        AsyncCoroutine.ExecuteAsyncCoroutine(shakeCamera(_mainCamera, timeForShake));  
    }

    public void OnNotify(AsyncCoroutine data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        AsyncCoroutine = data;
    }

    public void OnNotify(GenericStateBundle<EmitAnimationStateBundle> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        EmitAnimationStateBundle.StateBundle = data.StateBundle;

        StartCoroutine(ExecuteShakeAnimation(EmitAnimationStateBundle.StateBundle));
    }
}
