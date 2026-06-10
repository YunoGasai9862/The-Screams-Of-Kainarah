using Assets.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Annotations.Enums;
using Assets.Scripts.Scene;

[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(CandleLightPackageGenerator), ContextType = typeof(LightPackage))]
[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(CandleLightPackageGenerator), SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(Player))]
[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(CandleLightPackageGenerator), SubjectType = typeof(LightFlicker), ContextType = typeof(ILightPreprocess))]
public class CandleLightPackageGenerator : Scene, Assets.Scripts.Interfaces.Mediator.EnhancedV3.IRequest<LightPackage>, INotify<ILightPreprocess>, INotify<Player>, ILightPackageGenerator
{
    [SerializeField]
    LightProperties lightProperties;
    [SerializeField]
    float minDistanceFromPlayerForLightFlicker;
    [SerializeField]
    float delayBetweenExecution;

    private Delegator Delegator { get; set; }

    private ILightPreprocess LightPreprocess { get; set; }

    private Light2D LightSource { get; set; }

    private Player Player { get; set; }

    private SemaphoreSlim SemaphoreSlim { get; set; }

    private CancellationToken CancellationToken { get; set; }

    private CancellationTokenSource CancellationTokenSource { get; set; }

    private async void Start()
    {
        LightSource = GetComponent<Light2D>();

        SceneUtils.ValidateLightSourcePresence(LightSource);

        SemaphoreSlim = new SemaphoreSlim(1, 1);

        await SetupCancellationTokens();

        StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));

        Delegator.NotifySubjectWrapper(new ObserverContext<ILightPreprocess>()
        {
            Instance = gameObject,
            EntityType = typeof(CandleLightPackageGenerator),
            SubjectType = typeof(LightFlicker)
        }, this);

        Delegator.NotifySubjectWrapper(new ObserverContext<Player>()
        {
            Instance = gameObject,
            EntityType = typeof(CandleLightPackageGenerator),
            SubjectType = typeof(PlayerAttributesNotifier)
         }, this);
    }

    public IEnumerator PingCustomLightning(LightPackage lightPackage, INotify<LightPackage> observer, float delayPerExecutionInSeconds = 1)
    {
        while (true) 
        {
            lightPackage.LightSemaphore.WaitAsync(); //take the semaphore (will be released by the custom lightning class)

            lightPackage.LightProperties.ShouldLightPulse = Vector2.Distance(Player.Transform.position, gameObject.transform.position) < minDistanceFromPlayerForLightFlicker ? true : false;

            StartCoroutine(Delegator.NotifyObserver(new SubjectContext<LightPackage>() { Data = lightPackage, EntityType = typeof(CandleLightPackageGenerator) }, this, observer));

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

    private IEnumerator PrepareDataForCustomLightningGeneration(INotify<LightPackage> observer)
    {
        yield return new WaitUntil(() => IsReadyToCustomLightningEntity());

        StartCoroutine(PingCustomLightning(PrepareLightPackage(), observer, delayBetweenExecution));
    }

    private async Task SetupCancellationTokens()
    {
        CancellationTokenSource = new CancellationTokenSource();

        CancellationToken = CancellationTokenSource.Token;
    }

    private bool IsReadyToCustomLightningEntity()
    {
        return !SceneUtils.AreObjectsNull(new List<UnityEngine.Object>
        {
            Delegator
        })
            && LightPreprocess != null
            && Player != null;
    }

    public IEnumerator Notify(ILightPreprocess value)
    {
        LightPreprocess = value;

        yield return null;
    }

    public IEnumerator Notify(Player value)
    {
        Player = value;

        yield return null;
    }

    public Task<LightPackage> Request(INotify<LightPackage> obsever)
    {
        StartCoroutine(PrepareDataForCustomLightningGeneration(obsever));

        return null;
    }
}