using System.Threading;
using UnityEngine;

public class CelestialBodiesLightPackageGenerator : MonoBehaviour, IObserver<LightPackage>, IObserver<LightFlicker, ILightPreprocess>
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;
    LightPreprocessDelegatorManager lightPreprocessDelegatorManager;

    private ILightPreprocess lightFlickerPreprocess;

    private string LightFlickerUniqueKey { get; set; }

    private void Start()
    {
    }

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

        //CelestialBodiesLightPackageGenerator can be casted to Monobehavior since it inherits from it
        //just be aware that the observer gets it properly
        StartCoroutine(lightPreprocessDelegatorManager.LightPreprocessDelegator.NotifySubject(LightFlickerUniqueKey , (IObserver<MonoBehaviour, ILightPreprocess>)this));
    }
}