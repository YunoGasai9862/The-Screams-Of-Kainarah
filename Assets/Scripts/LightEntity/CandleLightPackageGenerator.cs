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

    private void Start()
    {
        LightSource = GetComponent<Light2D>();

        ValidateLightSourcePresence(LightSource);

        //notify light Preprocess
        StartCoroutine(lightPreprocessDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(LightFlicker).ToString()
        }));

        //notify the player
        StartCoroutine(playerAttributesDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()
         }));

        //act as a subject for lightpackage!
        lightPackageDelegator.AddToSubjectsDict(gameObject.tag, new Subject<IObserver<LightPackage>>() { });

        lightPackageDelegator.GetSubject(gameObject.tag).SetSubject(this);
    }

    //start calculating distance recursively - however with the aid of the player
    private IEnumerator CalculateDistanceFromPlayer(LightPackage lightPackage)
    {
        yield return null;
    }

    private LightPackage PrepareLightPackage()
    {
        return new LightPackage()
        {
            LightPreprocess = LightPreprocess,
            LightSource = LightSource,
            LightProperties = LightProperties.FromDefault(gameObject.name, true)
        };
    }

    private IEnumerator PrepareDataForCustomLightningGeneration()
    {
        yield return new WaitUntil(() => lightPreprocessDelegator != null);

        StartCoroutine(CalculateDistanceFromPlayer(PrepareLightPackage()));
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

    public void OnNotify(Transform data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        //now you have player's data, calculate the distance!
        Debug.Log($"Player's Transform: {data}");
    }
}