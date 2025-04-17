using System.Collections;
using System.Threading;
using UnityEngine;

[ObserverSystem(SubjectType = typeof(CandleLightPackageGenerator), ObserverType = typeof(CustomLightProcessing))]
[ObserverSystem(SubjectType = typeof(CelestialBodiesLightPackageGenerator), ObserverType = typeof(CustomLightProcessing))]
public class CustomLightProcessing : MonoBehaviour, ICustomLightPreprocessing, IObserver<AsyncCoroutine>, IObserver<LightPackage>
{
    private AsyncCoroutine AsyncCoroutine { get; set; }

    [Header("Light Intensity Swing Values")]
    [SerializeField]
    public float maxIntensity;
    public float minIntensity;

    [Header("Async Coroutine Delegator Reference")]
    [SerializeField]
    public AsyncCoroutineDelegator asyncCoroutineDelegator;

    [Header("LightPackage Coroutine Delegator Reference")]
    [SerializeField]
    public LightPackageDelegator lightPackageDelegator;

    private void Start()
    {
        StartCoroutine(asyncCoroutineDelegator.NotifySubject(this));

        StartCoroutine(lightPackageDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(CandleLightPackageGenerator).ToString())));
        StartCoroutine(lightPackageDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(CelestialBodiesLightPackageGenerator).ToString())));
    }

    public IEnumerator ExecuteLightningLogic(LightPackage lightPackage, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken)
    {
        yield return new WaitUntil(() => AsyncCoroutine != null);

        if (lightPackage != null)
        {
            AsyncCoroutine.ExecuteAsyncCoroutine(lightPackage.LightPreprocess.GenerateCustomLighting(lightPackage, semaphoreSlim, 5f)); //Async runner
        }
    }

    public void OnNotify(AsyncCoroutine data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        AsyncCoroutine = data;
    }

    public void OnNotify(LightPackage data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        StartCoroutine(ExecuteLightningLogic(data, data.LightSemaphore, data.CancellationToken));
    }
}
