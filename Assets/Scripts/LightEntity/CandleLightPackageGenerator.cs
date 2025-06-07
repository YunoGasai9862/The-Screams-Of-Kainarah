using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class CandleLightPackageGenerator : MonoBehaviour, ISubject<IObserver<LightPackage>>, IObserver<ILightPreprocess>, IObserver<Player>, ILightPackageGenerator
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;
    [SerializeField]
    LightPreprocessDelegator lightPreprocessDelegator;
    [SerializeField]
    PlayerAttributesDelegator playerAttributesDelegator;
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

        StartCoroutine(lightPreprocessDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(LightFlicker).ToString()
        }, CancellationToken.None));

        StartCoroutine(playerAttributesDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()
         }, CancellationToken.None));

        lightPackageDelegator.AddToSubjectsDict(typeof(CandleLightPackageGenerator).ToString(), transform.parent.gameObject.name, new Subject<IObserver<LightPackage>>() { });

        lightPackageDelegator.GetSubsetSubjectsDictionary(typeof(CandleLightPackageGenerator).ToString())[transform.parent.gameObject.name].SetSubject(this);
    }

    public IEnumerator PingCustomLightning(LightPackage lightPackage, IObserver<LightPackage> observer, float delayPerExecutionInSeconds = 1)
    {
        while (true) 
        {
            lightPackage.LightSemaphore.WaitAsync(); //take the semaphore (will be released by the custom lightning class)

            lightPackage.LightProperties.ShouldLightPulse = Vector2.Distance(Player.Transform.position, gameObject.transform.position) < minDistanceFromPlayerForLightFlicker ? true : false;

            StartCoroutine(lightPackageDelegator.NotifyObserver(observer, lightPackage, new NotificationContext()
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
            lightPreprocessDelegator, Player.Transform
        })
            && LightPreprocess != null;
    }
}