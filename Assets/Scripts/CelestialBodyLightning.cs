using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[ObserverSystem(SubjectType = typeof(CelestialBodyLightning), ObserverType = typeof(CelestialBodiesLightPackageGenerator))]

public class CelestialBodyLightning : MonoBehaviour, ILightPreprocess, ISubject<IObserver<ILightPreprocess>>
{
    [SerializeField]
    LightPreprocessDelegator lightPreprocessDelegator;

    private void Start()
    {
        lightPreprocessDelegator.AddToSubjectsDict(gameObject.tag, new Subject<IObserver<ILightPreprocess>>());

        lightPreprocessDelegator.GetSubject(gameObject.tag).SetSubject(this);
    }

    public void ActivateContinuousShimmer(Light2D light, float time, float minIntensity, float maxIntensity, float minOuterRadius, float maxOuterRadius, float minInnerRadius, float maxInnerRadius)
    {
        light.intensity = Mathf.PingPong(time, maxIntensity) + (minIntensity);
        light.pointLightOuterRadius = Mathf.PingPong(time * 2, maxOuterRadius) + minOuterRadius;
        light.pointLightInnerRadius = Mathf.PingPong(time * 2, maxInnerRadius) + minInnerRadius;
    }

    public async IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(LightPackage lightPackage, SemaphoreSlim couroutineBlocker, float delayBetweenExecution = 0)
    {
        ActivateContinuousShimmer(lightPackage.LightSource, Time.time, lightPackage.LightProperties.MinLightIntensity, lightPackage.LightProperties.MaxLightIntensity, lightPackage.LightProperties.OuterRadiusMin, lightPackage.LightProperties.OuterRadiusMax, lightPackage.LightProperties.InnerRadiusMin, lightPackage.LightProperties.InnerRadiusMax);
        await Task.Delay(TimeSpan.FromMilliseconds(0));

        //release it here
        couroutineBlocker.Release();
        yield return new WaitForSeconds(0);
    }

    public void OnNotifySubject(IObserver<ILightPreprocess> data, NotificationContext notificationContext, params object[] optional)
    {
        StartCoroutine(lightPreprocessDelegator.NotifyObserver(data, this, notificationContext));
    }
}
