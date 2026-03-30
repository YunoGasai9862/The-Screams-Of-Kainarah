
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Observer(AssetType = Annotations.Enums.Asset.MONOBEHAVIOR, ObserverType = typeof(ResetController), SubjectType = typeof(AsyncCoroutine), ContextType = typeof(AsyncCoroutine))]
[Observer(AssetType = Annotations.Enums.Asset.MONOBEHAVIOR, ObserverType = typeof(ResetController), SubjectType = typeof(EmitAttackAnimationStateConsumer), ContextType = typeof(GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState>))]
[Subject(AssetType = Annotations.Enums.Asset.MONOBEHAVIOR, SubjectType = typeof(CameraShake), ContextType = typeof(bool))]
public class CameraShake : MonoBehaviour, Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<AsyncCoroutine>, Assets.Scripts.Interfaces.Mediator.EnhancedV1.INotify<GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState>>, IRequest<bool>
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

    private Delegator Delegator { get; set; }
    private Vector3 CameraOldPosition { get; set; }

    private AsyncCoroutine AsyncCoroutine { get; set; }

    private GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState> StateBundle { get; set; } = new GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState>();

    private async void Start()
    {
        Delegator = await Helper.GetDelegator<Delegator>();

        Delegator.NotifySubjectWrapper(Helper.BuildNotificationContext<AsyncCoroutine>(gameObject, typeof(AsyncCoroutine)), this);

        Delegator.NotifySubjectWrapper(Helper.BuildNotificationContext<GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState>>(gameObject, typeof(EmitAttackAnimationStateConsumer)), this);
    }

    private async IAsyncEnumerator<WaitForSeconds> ShakeCamera(Camera _mainCamera, float timeForCameraShake)
    {
        float timeSpent = 0f;

        CameraOldPosition = _mainCamera.transform.position;

        StartCoroutine(Delegator.NotifyObservers(new SubjectContext<bool>() { Data = true, EntityType = typeof(CameraShake) }, this));

        while (timeSpent < timeForCameraShake)
        {
            mainCamera.transform.position = CameraOldPosition + new Vector3(UnityEngine.Random.Range(minShake, maxShake), UnityEngine.Random.Range(minShake, maxShake), 0);

            timeSpent += Time.deltaTime;

            await Task.Delay(TimeSpan.FromSeconds(delay));
        }

        mainCamera.transform.position = CameraOldPosition;

        StartCoroutine(Delegator.NotifyObservers(new SubjectContext<bool>() { Data = true, EntityType = typeof(CameraShake) }, this));

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


    public IEnumerator Notify(AsyncCoroutine value)
    {
        AsyncCoroutine = value;

        yield return null;
    }

    public IEnumerator Notify(GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState> value)
    {
        StateBundle.StateBundle = value.StateBundle;

        yield return StartCoroutine(ExecuteShakeAnimation(StateBundle.StateBundle));
    }
}
