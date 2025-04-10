using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[ObserverSystem(SubjectType = typeof(LightFlicker), ObserverType = typeof(CandleLightPackageGenerator))]
[ObserverSystem(SubjectType = typeof(CandleLightPackageGenerator), ObserverType = typeof(CustomLightProcessing))]
public class CandleLightPackageGenerator : MonoBehaviour, ISubject<IObserver<LightPackage>>, IObserver<ILightPreprocess>
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;
    [SerializeField]
    LightPreprocessDelegator lightPreprocessDelegator;


    private ILightPreprocess LightPreprocess { get; set; }

    private string LightFlickerUniqueKey { get; set; }

    private bool CustomPreprocessingScriptIsAlive { get; set;}

    private Light2D LightSource { get; set; }

    private void Start()
    {
        LightSource = GetComponent<Light2D>();

        ValidateLightSourcePresence(LightSource);

        StartCoroutine(lightPreprocessDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(LightFlicker).ToString()
        }));

        lightPackageDelegator.AddToSubjectsDict(gameObject.tag, new Subject<IObserver<LightPackage>>() { });

        lightPackageDelegator.GetSubject(gameObject.tag).SetSubject(this);
    }

    //start calculating distance recursively - however with the aid of the player
    private IEnumerator CalculateDistance(LightPackage lightPackage)
    {
        yield return null;
    }

    private LightPackage PrepareLightPackage()
    {
        return new LightPackage()
        {
            LightPreprocess = LightPreprocess,
            LightSource = LightSource,
            LightProperties = new LightProperties() // fill this out
        };
    }

    private IEnumerator PrepareDataForCustomLightningGeneration()
    {
        yield return new WaitUntil(() => lightPreprocessDelegator != null);

        //first prepare the initial light data
        LightPackage lightPackage = PrepareLightPackage();

        //then start the coroutine for Calculating Distance - this will be linked with Player's data
    }

    public void OnNotify(ILightPreprocess data, NotificationContext context, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        LightPreprocess = data;
    }

    public void OnNotifySubject(IObserver<LightPackage> data, NotificationContext notificationContext, params object[] optional)
    {
        StartCoroutine(PrepareDataForCustomLightningGeneration());
    }

    private void ValidateLightSourcePresence(Light2D light2D)
    {
        if (light2D == null)
        {
            throw new ApplicationException("LightSource is not Present!");
        }
    }
}