using System;
using System.Threading;
using UnityEngine;

[ObserverSystem(SubjectType = typeof(CelestialBodyLightning), ObserverType = typeof(CelestialBodiesLightPackageGenerator))]
public class CelestialBodiesLightPackageGenerator : MonoBehaviour, IObserver<ILightPreprocess>, IObserver<LightPackage>
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;
    LightPreprocessDelegator lightPreprocessDelegator;

    private ILightPreprocess celestialBodyLightningPreprocess;

    private void Start()
    {
        StartCoroutine(lightPreprocessDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(CelestialBodyLightning).ToString()
        }));
    }

    public void OnNotify(LightPackage data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        throw new System.NotImplementedException();
    }

    public void OnNotify(ILightPreprocess data, NotificationContext context, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        celestialBodyLightningPreprocess = data;
    }
}