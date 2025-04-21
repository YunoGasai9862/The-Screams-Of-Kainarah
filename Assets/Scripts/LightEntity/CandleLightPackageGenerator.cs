using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[ObserverSystem(SubjectType = typeof(LightFlicker), ObserverType = typeof(CandleLightPackageGenerator))]
[ObserverSystem(SubjectType = typeof(CandleLightPackageGenerator), ObserverType = typeof(CustomLightProcessing))]
[ObserverSystem(SubjectType = typeof(PlayerAttributesNotifier), ObserverType = typeof(CandleLightPackageGenerator))]
public class CandleLightPackageGenerator : MonoBehaviour, ISubject<IObserver<LightPackage>>, IObserver<ILightPreprocess>, IObserver<Transform>
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;
    [SerializeField]
    LightPreprocessDelegator lightPreprocessDelegator;
    [SerializeField]
    PlayerAttributesDelegator playerAttributesDelegator;

    private ILightPreprocess LightPreprocess { get; set; }

    private Light2D LightSource { get; set; }

    private Transform PlayersTransform { get; set; }

    private SemaphoreSlim SemaphoreSlim { get; set; }

    private CancellationToken CancellationToken { get; set; }

    private CancellationTokenSource CancellationTokenSource { get; set; }

    private const float MIN_DISTANCE = 5.0f;

    private async void Start()
    {
        LightSource = GetComponent<Light2D>();

        ValidateLightSourcePresence(LightSource);

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

        Debug.Log($"Turns out im overriding it: {transform.parent.gameObject.name}");
    }

    //start calculating distance recursively - however with the aid of the player
    private IEnumerator CalculateDistanceFromPlayer(LightPackage lightPackage, IObserver<LightPackage> observer, Transform playersTransform, float delayPerExecutionInSeconds = 1f)
    {
        while(true) //please have some sort of delay + termination condition. This usually hapepns in on update (for every frame)
        {
            //seems like candle 2 is not flickering - fix thsi!!

            lightPackage.LightSemaphore.WaitAsync(); //take the semaphore

            lightPackage.LightProperties.ShouldLightPulse = Vector2.Distance(playersTransform.transform.position, gameObject.transform.position) < MIN_DISTANCE ? true : false;

            StartCoroutine(lightPackageDelegator.NotifyObserver(observer, lightPackage, new NotificationContext()
            {
                SubjectType = typeof(CandleLightPackageGenerator).ToString(),
            }, lightPackage.CancellationToken));

            //unscaled yield (realTime) - waitForSeconds is scaled (RealTime wont stop if we set time.timeScale = 0)
            yield return new WaitForSeconds(delayPerExecutionInSeconds);
        }
    }

    private LightPackage PrepareLightPackage()
    {
        return new LightPackage()
        {
            LightPreprocess = LightPreprocess,
            LightSource = LightSource,
            LightProperties = LightProperties.FromDefault(gameObject.name, true),
            LightSemaphore = SemaphoreSlim,
            CancellationToken = CancellationToken,  
        };
    }

    private IEnumerator PrepareDataForCustomLightningGeneration(IObserver<LightPackage> observer)
    {
        yield return new WaitUntil(() => lightPreprocessDelegator != null);

        StartCoroutine(CalculateDistanceFromPlayer(PrepareLightPackage(), observer, PlayersTransform));
    }

    public void OnNotifySubject(IObserver<LightPackage> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(PrepareDataForCustomLightningGeneration(data));
    }

    private void ValidateLightSourcePresence(Light2D light2D)
    {
        if (light2D == null)
        {
            throw new ApplicationException("LightSource is not Present!");
        }
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

    public void OnNotify(Transform data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayersTransform = data;
    }

}