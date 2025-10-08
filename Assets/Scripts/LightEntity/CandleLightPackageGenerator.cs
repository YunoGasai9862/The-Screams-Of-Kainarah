using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class CandleLightPackageGenerator : MonoBehaviour, ISubject<IObserver<LightPackage>>, IObserver<ILightPreprocess>, IObserver<Player>, ILightPackageGenerator
{
    private LightPackageDelegator LightPackageDelegator { get; set; }
    private LightPreprocessDelegator LightPreprocessDelegator { get; set; }
    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }
    [SerializeField]
    LightProperties lightProperties;
    [SerializeField]
    float minDistanceFromPlayerForLightFlicker;
    [SerializeField]
    float delayBetweenExecution;

    private ILightPreprocess LightPreprocess { get; set; }

    private Light2D LightSource { get; set; }

    private Player Player { get; set; }

    private SemaphoreSlim SemaphoreSlim { get; set; }

    private CancellationToken CancellationToken { get; set; }

    private CancellationTokenSource CancellationTokenSource { get; set; }

    private async void Start()
    {
        LightSource = GetComponent<Light2D>();

        Helper.ValidateLightSourcePresence(LightSource);

        SemaphoreSlim = new SemaphoreSlim(1, 1);

        await SetupCancellationTokens();

        LightPackageDelegator = await Helper.GetDelegator<LightPackageDelegator>();

        LightPreprocessDelegator = await Helper.GetDelegator<LightPreprocessDelegator>();

        PlayerAttributesDelegator = await Helper.GetDelegator<PlayerAttributesDelegator>();

        LightPreprocessDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(LightFlicker).ToString()
        }, CancellationToken.None);

        PlayerAttributesDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()
         }, CancellationToken.None);

        LightPackageDelegator.AddToSubjectsDict(typeof(CandleLightPackageGenerator).ToString(), transform.parent.gameObject.name, new Subject<IObserver<LightPackage>>() { });

        LightPackageDelegator.GetSubsetSubjectsDictionary(typeof(CandleLightPackageGenerator).ToString())[transform.parent.gameObject.name].SetSubject(this);
    }

    public IEnumerator PingCustomLightning(LightPackage lightPackage, IObserver<LightPackage> observer, float delayPerExecutionInSeconds = 1)
    {
        while (true) 
        {
            lightPackage.LightSemaphore.WaitAsync(); //take the semaphore (will be released by the custom lightning class)

            lightPackage.LightProperties.ShouldLightPulse = Vector2.Distance(Player.Transform.position, gameObject.transform.position) < minDistanceFromPlayerForLightFlicker ? true : false;

            StartCoroutine(LightPackageDelegator.NotifyObserver(observer, lightPackage, new NotificationContext()
            {
                SubjectType = typeof(CandleLightPackageGenerator).ToString(),
            }, lightPackage.CancellationToken));

            //unscaled yield (realTime) - waitForSeconds is scaled (RealTime wont stop if we set time.timeScale = 0)
            yield return new WaitForSeconds(delayPerExecutionInSeconds/2);
        }
    }

    private LightPackage PrepareLightPackage()
    {
        return new LightPackage()
        {
            LightPreprocess = LightPreprocess,
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

    public void OnNotifySubject(IObserver<LightPackage> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
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
        LightPreprocess = data;
    }

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player = data;
    }

    private bool IsReadyToCustomLightningEntity()
    {
        return !Helper.AreObjectsNull(new List<UnityEngine.Object>
        {
            LightPreprocessDelegator
        })
            && LightPreprocess != null
            && Player != null;
    }
}