using System.Threading;
using UnityEngine;

public class CelestialBodiesLightPackageGenerator : MonoBehaviour, IObserver<LightPackage>, IObserver<LightFlicker, ILightPreprocess>
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;
    LightPreprocessDelegator<LightFlicker> lightPreprocessDelegator;

    private ILightPreprocess lightFlickerPreprocess;

    private string LightFlickerUniqueKey { get; set; }

    public void OnNotify(LightPackage data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        throw new System.NotImplementedException();
    }

    public void OnNotify(ILightPreprocess data, NotificationContext context, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        lightFlickerPreprocess = data;
    }

    public void OnKeyNotify(string key, NotificationContext context, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        LightFlickerUniqueKey = key;

        StartCoroutine(lightPreprocessDelegator.NotifySubject(LightFlickerUniqueKey , this));
    }
}