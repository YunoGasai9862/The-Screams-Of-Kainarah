using Assets.Annotations;
using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using Annotations.Enums;

[Observer(AssetType = Asset.MONOBEHAVIOR, ObserverType = typeof(CustomLightProcessing), SubjectType = typeof(CandleLightPackageGenerator), ContextType = typeof(LightPackage))]
[Observer(AssetType = Asset.MONOBEHAVIOR, ObserverType = typeof(CustomLightProcessing), SubjectType = typeof(CelestialBodiesLightPackageGenerator), ContextType = typeof(LightPackage))]
[Observer(AssetType = Asset.MONOBEHAVIOR, ObserverType = typeof(CustomLightProcessing), SubjectType = typeof(AsyncCoroutine), ContextType = typeof(AsyncCoroutine))]
public class CustomLightProcessing : MonoBehaviour, ICustomLightPreprocessing, INotify<AsyncCoroutine>, INotify<LightPackage>
{
    private AsyncCoroutine AsyncCoroutine { get; set; }

    [Header("Light Intensity Swing Values")]
    [SerializeField]
    public float maxIntensity;
    public float minIntensity;

    private Delegator Delegator { get; set; }

    private async void Start()
    {
        Delegator = await Helper.GetDelegator<Delegator>();

        Delegator.NotifySubjectWrapper(Helper.BuildNotificationContext<AsyncCoroutine>(gameObject, typeof(AsyncCoroutine), typeof(CustomLightProcessing)), this);
        Delegator.NotifySubjectWrapper(Helper.BuildNotificationContext<LightPackage>(gameObject, typeof(CandleLightPackageGenerator), typeof(CustomLightProcessing)), this);
        Delegator.NotifySubjectWrapper(Helper.BuildNotificationContext<LightPackage>(gameObject, typeof(CelestialBodiesLightPackageGenerator), typeof(CustomLightProcessing)), this);
    }

    public IEnumerator ExecuteLightningLogic(LightPackage lightPackage)
    {
        yield return new WaitUntil(() => AsyncCoroutine != null);

        if (lightPackage != null && !lightPackage.CancellationToken.IsCancellationRequested)
        {
            AsyncCoroutine.ExecuteAsyncCoroutine(lightPackage.LightPreprocess.GenerateCustomLighting(lightPackage));
        }
    }

    public IEnumerator Notify(LightPackage value)
    {
        yield return StartCoroutine(ExecuteLightningLogic(value));
    }

    public IEnumerator Notify(AsyncCoroutine value)
    {
        AsyncCoroutine = value;

        yield return null;
    }
}
