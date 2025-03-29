using System.Threading;
using UnityEngine;

[ObserverSystem(SubjectType = typeof(LightFlicker), ObserverType = typeof(CandleLightPackageGenerator))]

public class CandleLightPackageGenerator : MonoBehaviour, IObserver<LightPackage>, IObserver<ILightPreprocess>
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;
    [SerializeField]
    LightPreprocessDelegator lightPreprocessDelegator;


    private ILightPreprocess lightFlickerPreprocess;

    private string LightFlickerUniqueKey { get; set; }

    private void Start()
    {
        StartCoroutine(lightPreprocessDelegator.NotifyWhenActive(this, new NotificationContext()
        {
            GameObjectName = gameObject.name,
            GameObjectTag = gameObject.tag,
        }));
    }

    private bool CalculateDistance()
    {
        return false;
    }

    private LightPackage PrepareLightPackage()
    {
        return null;
    }

    public void OnNotify(LightPackage data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        throw new System.NotImplementedException();
    }

    public void OnNotify(ILightPreprocess data, NotificationContext context, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        lightFlickerPreprocess = data;
    }

}