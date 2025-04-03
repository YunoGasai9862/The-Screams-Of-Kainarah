using System.Threading;
using UnityEngine;

[ObserverSystem(SubjectType = typeof(LightFlicker), ObserverType = typeof(CandleLightPackageGenerator))]
[ObserverSystem(SubjectType = typeof(CandleLightPackageGenerator), ObserverType = typeof(CustomLightProcessing))]
public class CandleLightPackageGenerator : MonoBehaviour, ISubject<IObserver<LightPackage>>, IObserver<ILightPreprocess>
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;
    [SerializeField]
    LightPreprocessDelegator lightPreprocessDelegator;


    private ILightPreprocess lightFlickerPreprocess;

    private string LightFlickerUniqueKey { get; set; }

    private void Start()
    {
        StartCoroutine(lightPreprocessDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(LightFlicker).ToString()
        }));

        lightPackageDelegator.AddToSubjectsDict(gameObject.tag, new Subject<IObserver<LightPackage>>() { });

        lightPackageDelegator.GetSubject(gameObject.tag).SetSubject(this);
    }

    private bool CalculateDistance()
    {
        return false;
    }

    private LightPackage PrepareLightPackage()
    {
        return null;
    }

    public void OnNotify(ILightPreprocess data, NotificationContext context, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        lightFlickerPreprocess = data;
    }

    public void OnNotifySubject(IObserver<LightPackage> data, NotificationContext notificationContext, params object[] optional)
    {
        throw new System.NotImplementedException();
    }
}