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
        lightPreprocessDelegator.AddToSubjectsDict(typeof(CelestialBodyLightning).ToString(), gameObject.name, new Subject<IObserver<ILightPreprocess>>());

        //now access the dictionary on the name and set the subject
        lightPreprocessDelegator.GetSubsetSubjectsDictionary(typeof(CelestialBodyLightning).ToString())[gameObject.name].SetSubject(this);
    }

    //check how you wanna make it in a loop
    public async Task ActivateContinuousShimmer(Light2D light, float time, float minIntensity, float maxIntensity, float minOuterRadius, float maxOuterRadius, float minInnerRadius, float maxInnerRadius)
    {
        light.intensity = Mathf.PingPong(time, maxIntensity) + (minIntensity);
        light.pointLightOuterRadius = Mathf.PingPong(time * 2, maxOuterRadius) + minOuterRadius;
        light.pointLightInnerRadius = Mathf.PingPong(time * 2, maxInnerRadius) + minInnerRadius;
    }

    public async IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(LightPackage lightPackage, float delayBetweenExecution = 0)
    {
        await ActivateContinuousShimmer(lightPackage.LightSource, Time.time, lightPackage.LightProperties.MinLightIntensity, lightPackage.LightProperties.MaxLightIntensity, lightPackage.LightProperties.OuterRadiusMin, lightPackage.LightProperties.OuterRadiusMax, lightPackage.LightProperties.InnerRadiusMin, lightPackage.LightProperties.InnerRadiusMax);

        //release it here
        lightPackage.LightSemaphore.Release();
        yield return null;
    }

    public void OnNotifySubject(IObserver<ILightPreprocess> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(lightPreprocessDelegator.NotifyObserver(data, this, notificationContext, cancellationToken, semaphoreSlim));
    }
}
