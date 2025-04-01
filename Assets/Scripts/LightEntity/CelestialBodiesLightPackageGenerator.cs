using System;
using System.Threading;
using UnityEngine;

[ObserverSystem(SubjectType = typeof(CelestialBodyLightning), ObserverType = typeof(CelestialBodiesLightPackageGenerator))]
[ObserverSystem(SubjectType = typeof(CelestialBodiesLightPackageGenerator), ObserverType = typeof(CustomLightProcessing))]
public class CelestialBodiesLightPackageGenerator : MonoBehaviour, IObserver<ILightPreprocess>, ISubject<IObserver<LightPackage>>
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

    public void OnNotify(ILightPreprocess data, NotificationContext context, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        celestialBodyLightningPreprocess = data;
    }

    public void OnNotifySubject(IObserver<LightPackage> data, NotificationContext notificationContext, params object[] optional)
    {
        throw new NotImplementedException();
    }
}