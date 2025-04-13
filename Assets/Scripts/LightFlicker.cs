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
        lightPreprocessDelegator.AddToSubjectsDict(typeof(LightFlicker).ToString(), new Subject<IObserver<ILightPreprocess>>());

        lightPreprocessDelegator.GetSubject(typeof(LightFlicker).ToString()).SetSubject(this);
    }

    public async IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(LightPackage lightPackage, SemaphoreSlim couroutineBlocker, float delayBetweenExecution = 0)
    {
        lightPackage.LightSource.intensity = await GenerateLightRadia(lightPackage.LightProperties.OuterRadiusMin, lightPackage.LightProperties.OuterRadiusMax);
        lightPackage.LightSource.pointLightInnerRadius = await GenerateLightRadia(lightPackage.LightProperties.InnerRadiusMin, lightPackage.LightProperties.InnerRadiusMax);
        lightPackage.LightSource.pointLightOuterRadius = await GenerateLightIntensityAsync(lightPackage.LightProperties.MinLightIntensity, lightPackage.LightProperties.MaxLightIntensity);

        couroutineBlocker.Release();

        await Task.Delay(TimeSpan.FromSeconds(delayBetweenExecution));

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

    public void OnNotifySubject(IObserver<ILightPreprocess> data, NotificationContext notificationContext, params object[] optional)
    {
        StartCoroutine(lightPreprocessDelegator.NotifyObserver(data, this, notificationContext));
    }
}
