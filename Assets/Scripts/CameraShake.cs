
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CameraShake : MonoBehaviour, IObserver<AsyncCoroutine>, IObserver<GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState>>, ISubject<bool>
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

    private AsyncCoroutineDelegator AsyncCoroutineDelegator { get; set; }

    private EmitAnimationAttackStateDelegator EmitAnimationAttackStateDelegator { get; set; }

    private FlagDelegator FlagDelegator { get; set; }

    private Vector3 CameraOldPosition { get; set; }

    private AsyncCoroutine AsyncCoroutine { get; set; }

    private GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState> StateBundle { get; set; } = new GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState>();

    private async void Start()
    {
        AsyncCoroutineDelegator = await Helper.GetDelegator<AsyncCoroutineDelegator>();

        EmitAnimationAttackStateDelegator = await Helper.GetDelegator<EmitAnimationAttackStateDelegator>();

        FlagDelegator = await Helper.GetDelegator<FlagDelegator>();

        AsyncCoroutineDelegator.NotifySubjectWrapper(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(AsyncCoroutine).ToString()), CancellationToken.None);

        EmitAnimationAttackStateDelegator.NotifySubjectWrapper(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(EmitAttackAnimationStateConsumer).ToString()), CancellationToken.None);

        FlagDelegator.AddToSubjectsDict(typeof(CameraShake).ToString(), name, new Subject<bool>(this, typeof(CameraShake)));
    }

    private async IAsyncEnumerator<WaitForSeconds> ShakeCamera(Camera _mainCamera, float timeForCameraShake)
    {
        float timeSpent = 0f;

        CameraOldPosition = _mainCamera.transform.position;

        FlagDelegator.NotifyObservers(true, name, CancellationToken.None);

        while (timeSpent < timeForCameraShake)
        {
            mainCamera.transform.position = CameraOldPosition + new Vector3(UnityEngine.Random.Range(minShake, maxShake), UnityEngine.Random.Range(minShake, maxShake), 0);

            timeSpent += Time.deltaTime;

            await Task.Delay(TimeSpan.FromSeconds(delay));
        }

        mainCamera.transform.position = CameraOldPosition;

        FlagDelegator.NotifyObservers(true, name, CancellationToken.None);

        yield return new WaitForSeconds(0f);
    }

    private IEnumerator ExecuteShakeAnimation(EmitAnimationStateBundle<bool> stateBundle)
    {
        if (!stateBundle.CurrentAnimation.CurrentValue)
        {
            yield return null;
        }

        yield return new WaitUntil(() => AsyncCoroutine != null);
        
        AsyncCoroutine.ExecuteAsyncCoroutine(ShakeCamera(mainCamera, timeForShake));  
    }

    public void OnNotify(AsyncCoroutine data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        AsyncCoroutine = data;
    }

    public void OnNotify(GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState> data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        StateBundle.StateBundle = data.StateBundle;

        StartCoroutine(ExecuteShakeAnimation(StateBundle.StateBundle));
    }

    public void OnNotifySubject(IObserver<bool> observer, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        FlagDelegator.CreateAssociation(typeof(CameraShake).ToString(), FlagDelegator.GetSubsetSubjectsDictionary(typeof(CameraShake).ToString())[gameObject.name], observer);
    }
}
