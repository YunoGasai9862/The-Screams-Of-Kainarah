using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[ObserverSystem(SubjectType = typeof(LightFlicker), ObserverType = typeof(CandleLightPackageGenerator))]

public class LightFlicker : MonoBehaviour, ILightPreprocess, ISubject<IObserver<ILightPreprocess>>
{
    [SerializeField]
    LightPreprocessDelegator lightPreprocessDelegator;
    private void Start()
    {
        lightPreprocessDelegator.AddToSubjectsDict(typeof(LightFlicker).ToString(), gameObject.name, new Subject<IObserver<ILightPreprocess>>());

        lightPreprocessDelegator.GetSubsetSubjectsDictionary(typeof(LightFlicker).ToString())[gameObject.name].SetSubject(this);
    }

    public async IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(LightPackage lightPackage, float delayBetweenExecution = 0)
    {
        lightPackage.LightSource.intensity = lightPackage.LightProperties.ShouldLightPulse ?
            await GenerateLightIntensityAsync(lightPackage.LightProperties.MinLightIntensity, lightPackage.LightProperties.MaxLightIntensity) : lightPackage.LightSource.intensity;
        lightPackage.LightSource.pointLightInnerRadius = lightPackage.LightProperties.ShouldLightPulse?
            await GenerateLightRadia(lightPackage.LightProperties.InnerRadiusMin, lightPackage.LightProperties.InnerRadiusMax) : lightPackage.LightSource.pointLightInnerRadius;
        lightPackage.LightSource.pointLightOuterRadius = lightPackage.LightProperties.ShouldLightPulse ?
            await GenerateLightRadia(lightPackage.LightProperties.OuterRadiusMin, lightPackage.LightProperties.OuterRadiusMax) : lightPackage.LightSource.pointLightOuterRadius;

        lightPackage.LightSemaphore.Release();

        yield return null;
    }

    public Task<float> GenerateLightIntensityAsync(float minIntensity, float maxIntensity)
    {
        return Task.FromResult(UnityEngine.Random.Range(minIntensity, maxIntensity));
    }
    public Task<float> GenerateLightRadia(float minRadia, float maxRadia)
    {
        return Task.FromResult(UnityEngine.Random.Range(minRadia, maxRadia));
    }


    public void OnNotifySubject(IObserver<ILightPreprocess> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(lightPreprocessDelegator.NotifyObserver(data, this, notificationContext, cancellationToken, semaphoreSlim));
    }
}
