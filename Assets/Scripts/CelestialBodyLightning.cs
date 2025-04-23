using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[ObserverSystem(SubjectType = typeof(CelestialBodyLightning), ObserverType = typeof(CelestialBodiesLightPackageGenerator))]

public class CelestialBodyLightning : MonoBehaviour, ILightPreprocess, ISubject<IObserver<ILightPreprocess>>
{
    [SerializeField]
    LightPreprocessDelegator lightPreprocessDelegator;

    private void Start()
    {
        lightPreprocessDelegator.AddToSubjectsDict(typeof(CelestialBodyLightning).ToString(), gameObject.name, new Subject<IObserver<ILightPreprocess>>());

        lightPreprocessDelegator.GetSubsetSubjectsDictionary(typeof(CelestialBodyLightning).ToString())[gameObject.name].SetSubject(this);
    }

    public async IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(LightPackage lightPackage, float delayBetweenExecution = 0)
    {
        //revisit pingPong logic
        lightPackage.LightSource.intensity = Mathf.PingPong(Time.time, lightPackage.LightProperties.MaxLightIntensity) + (lightPackage.LightProperties.MinLightIntensity);
        lightPackage.LightSource.pointLightOuterRadius = Mathf.PingPong(Time.time, lightPackage.LightProperties.OuterRadiusMax) + lightPackage.LightProperties.OuterRadiusMin;
        lightPackage.LightSource.pointLightInnerRadius = Mathf.PingPong(Time.time, lightPackage.LightProperties.InnerRadiusMax) + lightPackage.LightProperties.InnerRadiusMin;

        lightPackage.LightSemaphore.Release();
        yield return null;
    }

    public void OnNotifySubject(IObserver<ILightPreprocess> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(lightPreprocessDelegator.NotifyObserver(data, this, notificationContext, cancellationToken, semaphoreSlim));
    }
}
