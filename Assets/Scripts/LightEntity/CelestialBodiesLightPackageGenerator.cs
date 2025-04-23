using System;
using System.Collections;
using System.Threading;
using UnityEngine;

[ObserverSystem(SubjectType = typeof(CelestialBodyLightning), ObserverType = typeof(CelestialBodiesLightPackageGenerator))]
[ObserverSystem(SubjectType = typeof(CelestialBodiesLightPackageGenerator), ObserverType = typeof(CustomLightProcessing))]
public class CelestialBodiesLightPackageGenerator : MonoBehaviour, IObserver<ILightPreprocess>, ISubject<IObserver<LightPackage>>, ILightPackageGenerator
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;
    [SerializeField]
    LightPreprocessDelegator lightPreprocessDelegator;

    private ILightPreprocess celestialBodyLightningPreprocess;

    private void Start()
    {

        StartCoroutine(lightPreprocessDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(CelestialBodyLightning).ToString()
        }, CancellationToken.None));

        //subject for custom lightning
        lightPackageDelegator.AddToSubjectsDict(typeof(CelestialBodiesLightPackageGenerator).ToString(), gameObject.name, new Subject<IObserver<LightPackage>>());

        lightPackageDelegator.GetSubsetSubjectsDictionary(typeof(CelestialBodiesLightPackageGenerator).ToString())[gameObject.name].SetSubject(this);
    }

    public void OnNotifySubject(IObserver<LightPackage> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {

    }

    public void OnNotify(ILightPreprocess data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        celestialBodyLightningPreprocess = data;
    }

    public IEnumerator PingCustomLightning(LightPackage lightPackage, IObserver<LightPackage> observer, float delayPerExecutionInSeconds = 1)
    {
        throw new NotImplementedException();
    }
}