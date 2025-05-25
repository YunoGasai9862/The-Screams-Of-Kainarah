using System.Collections;
using System.Threading;
using UnityEngine;

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
        StartCoroutine(asyncCoroutineDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(AsyncCoroutine).ToString()), CancellationToken.None));

        StartCoroutine(lightPackageDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(CandleLightPackageGenerator).ToString()), CancellationToken.None));
        StartCoroutine(lightPackageDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(CelestialBodiesLightPackageGenerator).ToString()), CancellationToken.None));
    }

    public IEnumerator ExecuteLightningLogic(LightPackage lightPackage)
    {
        yield return new WaitUntil(() => AsyncCoroutine != null);

        if (lightPackage != null && !lightPackage.CancellationToken.IsCancellationRequested)
        {
            AsyncCoroutine.ExecuteAsyncCoroutine(lightPackage.LightPreprocess.GenerateCustomLighting(lightPackage)); //Async runner
        }
    }

    public void OnNotify(AsyncCoroutine data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        AsyncCoroutine = data;
    }

    public void OnNotify(LightPackage data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        StartCoroutine(ExecuteLightningLogic(data));
    }
}
