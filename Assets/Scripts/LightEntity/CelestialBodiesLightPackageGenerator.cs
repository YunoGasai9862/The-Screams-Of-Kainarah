using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using Assets.Scripts.BaseScene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(CelestialBodiesLightPackageGenerator), ContextType = typeof(LightPackage))]
[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(CelestialBodiesLightPackageGenerator), SubjectType = typeof(CelestialBodyLightning), ContextType = typeof(ILightPreprocess))]
public class CelestialBodiesLightPackageGenerator : MonoBehaviorScene, INotify<ILightPreprocess>, Assets.Scripts.Interfaces.Mediator.EnhancedV4.IRequest<LightPackage>, ILightPackageGenerator
{
    private Delegator Delegator { get; set; }

    [SerializeField]
    LightProperties lightProperties;
    [SerializeField]
    float delayBetweenExecution;

    private ILightPreprocess CelestialLightningLightPreprocess { get; set; }

    private Light2D LightSource { get; set; }

    private SemaphoreSlim SemaphoreSlim { get; set; }

    private CancellationToken CancellationToken { get; set; }

    private CancellationTokenSource CancellationTokenSource { get; set; }

    private SceneUtils SceneUtils { get; set; }

    private async void Start()
    {

        SceneUtils = await (await GetBaseScene()).GetSceneUtilsAsync();

        LightSource = GetComponent<Light2D>();

        bool isValid = SceneUtils.IsLightSourceValid(LightSource);

        if (!isValid)
        {
            throw new ApplicationException("LightSource is not Present!");
        }

        SemaphoreSlim = new SemaphoreSlim(1, 1);

        await SetupCancellationTokens();

        Delegator = SceneUtils.GetDelegator();

        Delegator.NotifySubjectWrapper(new ObserverContext<ILightPreprocess>()
        {
            Instance = gameObject,
            EntityType = typeof(CelestialBodiesLightPackageGenerator),
            SubjectType = typeof(CelestialBodyLightning)
        }, this);
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

    public async void OnNotifySubject(IObserver<LightPackage> data, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
    }

    private async Task SetupCancellationTokens()
    {
        CancellationTokenSource = new CancellationTokenSource();

        CancellationToken = CancellationTokenSource.Token;
    }

    public IEnumerator PingCustomLightning(LightPackage lightPackage, INotify<LightPackage> observer, float delayPerExecutionInSeconds = 1)
    {
        while(true)
        {
            lightPackage.LightSemaphore.WaitAsync();

            StartCoroutine(Delegator.NotifyObserver(new SubjectContext<LightPackage>()
            {
                Data = lightPackage,
                EntityType = typeof(CelestialBodiesLightPackageGenerator)
            }, this, observer));


            yield return new WaitForSeconds(delayPerExecutionInSeconds);
        }
    }

    private bool IsReadyToCustomLightningEntity()
    {
        return !SceneUtils.AreObjectsNull(new List<UnityEngine.Object>
        {
            Delegator
        })
            && CelestialLightningLightPreprocess != null;
    }

    public IEnumerator Request(INotify<LightPackage> obsever)
    {
        yield return new WaitUntil(() => IsReadyToCustomLightningEntity());

        StartCoroutine(PingCustomLightning(PrepareLightPackage(), obsever, delayBetweenExecution));
    }

    public IEnumerator Notify(ILightPreprocess value)
    {
        CelestialLightningLightPreprocess = value;

        yield return null;
    }
}