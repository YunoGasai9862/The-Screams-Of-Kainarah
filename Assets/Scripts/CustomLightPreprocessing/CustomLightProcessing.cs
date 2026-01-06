using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CustomLightProcessing : MonoBehaviour, ICustomLightPreprocessing, IObserver<AsyncCoroutine>, IObserver<LightPackage>
{
    private AsyncCoroutine AsyncCoroutine { get; set; }

    [Header("Light Intensity Swing Values")]
    [SerializeField]
    public float maxIntensity;
    public float minIntensity;

    private AsyncCoroutineDelegator AsyncCoroutineDelegator { get; set; }

    private LightPackageDelegator LightPackageDelegator { get; set; }

    private async void Start()
    {
        LightPackageDelegator = await Helper.GetDelegator<LightPackageDelegator>();

        AsyncCoroutineDelegator = await Helper.GetDelegator<AsyncCoroutineDelegator>();

        AsyncCoroutineDelegator.NotifySubjectWrapper(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(AsyncCoroutine)), CancellationToken.None);
        LightPackageDelegator.NotifySubjectWrapper(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(CandleLightPackageGenerator)), CancellationToken.None);
        LightPackageDelegator.NotifySubjectWrapper(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(CelestialBodiesLightPackageGenerator)), CancellationToken.None);
    }

    public IEnumerator ExecuteLightningLogic(LightPackage lightPackage)
    {
        yield return new WaitUntil(() => AsyncCoroutine != null);

        if (lightPackage != null && !lightPackage.CancellationToken.IsCancellationRequested)
        {
            AsyncCoroutine.ExecuteAsyncCoroutine(lightPackage.LightPreprocess.GenerateCustomLighting(lightPackage)); //Async runner
        }
    }

    public void OnNotify(AsyncCoroutine data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        AsyncCoroutine = data;
    }

    public void OnNotify(LightPackage data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        StartCoroutine(ExecuteLightningLogic(data));
    }
}
