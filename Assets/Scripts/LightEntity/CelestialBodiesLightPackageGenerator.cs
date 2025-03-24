using System;
using System.Threading;
using UnityEngine;

[Subject(typeof(CelestialBodyLightning))]
public class CelestialBodiesLightPackageGenerator : MonoBehaviour, IObserverEnhanced<ILightPreprocess>, IObserver<LightPackage>
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;
    LightPreprocessDelegator lightPreprocessDelegator;

    private ILightPreprocess celestialBodyLightningPreprocess;

    private string CelestialBodyLightningUniqueKey { get; set; }

    private void Start()
    {
        StartCoroutine(lightPreprocessDelegator.NotifyWhenActive(this, new NotificationContext()
        {
            GameObject = gameObject,
            GameObjectName = gameObject.name,
            GameObjectTag = gameObject.tag,    
        }));
    }

    public void OnNotify(LightPackage data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        throw new System.NotImplementedException();
    }

    public void OnKeyNotify(string key, NotificationContext context, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        CelestialBodyLightningUniqueKey = key;

        //CelestialBodiesLightPackageGenerator can be casted to Monobehavior since it inherits from it
        //just be aware that the observer gets it properly
        StartCoroutine(lightPreprocessDelegator.NotifySubject(CelestialBodyLightningUniqueKey, this));
    }

    public void OnNotify(ILightPreprocess data, NotificationContext context, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        celestialBodyLightningPreprocess = data;
    }
}