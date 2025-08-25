
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CameraShake : MonoBehaviour, IObserver<AsyncCoroutine>, IObserver<GenericStateBundle<EmitAnimationStateBundle>>,ISubject<IObserver<bool>>
{
    [Header("Target Camera")]
    [SerializeField] Camera mainCamera;

    [Header("Shake Min and Max Range")]
    [SerializeField] float minShake;
    [SerializeField] float maxShake;

    [Header("Time for shake")]
    [SerializeField] float timeForShake; //0.03f (old)

    [Header("Delay Between Each Shake")]
    [SerializeField] float delay; //0.05f (old)

    [Header("Async Coroutine Delegator")]
    [SerializeField] AsyncCoroutineDelegator asyncCoroutineDelegator;

    [Header("Emit Animation Delegator")]
    [SerializeField] EmitAnimationStateDelegator emitAnimationStateDelegator;

    [Header("Flag Delegator")]
    [SerializeField] FlagDelegator flagDelegator;

    private Vector3 CameraOldPosition { get; set; }

    private AsyncCoroutine AsyncCoroutine { get; set; }

    private GenericStateBundle<EmitAnimationStateBundle> EmitAnimationStateBundle { get; set; } = new GenericStateBundle<EmitAnimationStateBundle>();

    private void Start()
    {
        StartCoroutine(asyncCoroutineDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(AsyncCoroutine).ToString()), CancellationToken.None));

        StartCoroutine(emitAnimationStateDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(EmitAnimationStateConsumer).ToString()), CancellationToken.None));
    }

    private async IAsyncEnumerator<WaitForSeconds> ShakeCamera(Camera _mainCamera, float timeForCameraShake)
    {
        float timeSpent = 0f;

        CameraOldPosition = _mainCamera.transform.position;

        flagDelegator.NotifyObservers(false, name, typeof(CameraShake), CancellationToken.None);

        while (timeSpent < timeForCameraShake)
        {
            mainCamera.transform.position = CameraOldPosition + new Vector3(UnityEngine.Random.Range(minShake, maxShake), UnityEngine.Random.Range(minShake, maxShake), 0);

            timeSpent += Time.deltaTime;

            await Task.Delay(TimeSpan.FromSeconds(delay));
        }

        mainCamera.transform.position = CameraOldPosition;

        flagDelegator.NotifyObservers(true, name, typeof(CameraShake), CancellationToken.None);

        yield return new WaitForSeconds(0f);
    }

    private IEnumerator ExecuteShakeAnimation(EmitAnimationStateBundle stateBundle)
    {
        if (!stateBundle.IsRunning)
        {
            yield return null;
        }

        yield return new WaitUntil(() => AsyncCoroutine != null);
        
        AsyncCoroutine.ExecuteAsyncCoroutine(ShakeCamera(mainCamera, timeForShake));  
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

    public void OnNotifySubject(IObserver<bool> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        flagDelegator.AddToSubjectObserversDict(typeof(CameraShake).ToString(), flagDelegator.GetSubsetSubjectsDictionary(typeof(CameraShake).ToString())[gameObject.name], observer);
    }
}
