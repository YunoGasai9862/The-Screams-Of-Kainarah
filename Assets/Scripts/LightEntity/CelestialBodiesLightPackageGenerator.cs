using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class CelestialBodiesLightPackageGenerator : MonoBehaviour, IObserver<ILightPreprocess>, ISubject<LightPackage>, ILightPackageGenerator
{
    private LightPackageDelegator LightPackageDelegator { get; set; }

    private LightPreprocessDelegator LightPreprocessDelegator { get; set; }

    [SerializeField]
    LightProperties lightProperties;
    [SerializeField]
    float delayBetweenExecution;

    private ILightPreprocess CelestialLightningLightPreprocess { get; set; }

    private Light2D LightSource { get; set; }

    private SemaphoreSlim SemaphoreSlim { get; set; }

    private CancellationToken CancellationToken { get; set; }

    private CancellationTokenSource CancellationTokenSource { get; set; }

    private async void Start()
    {
        LightSource = GetComponent<Light2D>();

        Helper.ValidateLightSourcePresence(LightSource);

        SemaphoreSlim = new SemaphoreSlim(1, 1);

        await SetupCancellationTokens();

        LightPreprocessDelegator = await Helper.GetDelegator<LightPreprocessDelegator>();

        LightPackageDelegator = await Helper.GetDelegator<LightPackageDelegator>();

        LightPreprocessDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(CelestialBodyLightning).ToString()
        }, CancellationToken.None);

        //subject for custom lightning
        LightPackageDelegator.AddToSubjectsDict(typeof(CelestialBodiesLightPackageGenerator).ToString(), gameObject.name, new Subject<LightPackage>());

        LightPackageDelegator.GetSubsetSubjectsDictionary(typeof(CelestialBodiesLightPackageGenerator).ToString())[gameObject.name].SetSubject(this);
    }


    private LightPackage PrepareLightPackage()
    {
        return new LightPackage()
        {
            LightPreprocess = CelestialLightningLightPreprocess,
            LightSource = LightSource,
            LightProperties = lightProperties,
            LightSemaphore = SemaphoreSlim,
            CancellationToken = CancellationToken,
        };
    }

    private IEnumerator PrepareDataForCustomLightningGeneration(IObserver<LightPackage> observer)
    {
        yield return new WaitUntil(() => IsReadyToCustomLightningEntity());

        StartCoroutine(PingCustomLightning(PrepareLightPackage(), observer, delayBetweenExecution));
    }


    public async void OnNotifySubject(IObserver<LightPackage> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(PrepareDataForCustomLightningGeneration(data));
    }

    private async Task SetupCancellationTokens()
    {
        CancellationTokenSource = new CancellationTokenSource();

        CancellationToken = CancellationTokenSource.Token;
    }

    public void OnNotify(ILightPreprocess data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CelestialLightningLightPreprocess = data;
    }

    public IEnumerator PingCustomLightning(LightPackage lightPackage, IObserver<LightPackage> observer, float delayPerExecutionInSeconds = 1)
    {
        while(true)
        {
            lightPackage.LightSemaphore.WaitAsync();

            StartCoroutine(LightPackageDelegator.NotifyObserver(observer, lightPackage, new NotificationContext()
            {
                SubjectType = typeof(CelestialBodiesLightPackageGenerator).ToString()
            }, lightPackage.CancellationToken, lightPackage.LightSemaphore));


            yield return new WaitForSeconds(delayPerExecutionInSeconds);
        }
    }

    private bool IsReadyToCustomLightningEntity()
    {
        return !Helper.AreObjectsNull(new List<UnityEngine.Object>
        {
            LightPreprocessDelegator
        })
            && CelestialLightningLightPreprocess != null;
    }
}