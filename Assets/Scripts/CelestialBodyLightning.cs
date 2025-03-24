using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CelestialBodyLightning : MonoBehaviour, ILightPreprocess, ISubject<IObserverEnhanced<ILightPreprocess>>, ISubjectActivationNotifier<IObserverEnhanced<ILightPreprocess>>
{
    private const string CELESTIAL_BODY_LIGHTNING_SUBJECT_UNIQUE_IDENTIFIER = "celestial-body-lightning";

    [SerializeField]
    LightPreprocessDelegator lightPreprocessDelegator;

    private void Start()
    {
        lightPreprocessDelegator.SubjectsDict.Add(CELESTIAL_BODY_LIGHTNING_SUBJECT_UNIQUE_IDENTIFIER, new SubjectNotifier<IObserverEnhanced<ILightPreprocess>>() { });

        lightPreprocessDelegator.SubjectsDict[CELESTIAL_BODY_LIGHTNING_SUBJECT_UNIQUE_IDENTIFIER].SetSubject(this);

        ///yay now we dont need to cast even though monobehavior is expected, we are passing celestial body lightning.
        //this is beacuse of 'in' contravariance
        lightPreprocessDelegator.SubjectsDict[CELESTIAL_BODY_LIGHTNING_SUBJECT_UNIQUE_IDENTIFIER].SetSubjectActivationNotifier(this);

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

    public void NotifySubjectOfActivation(IObserverEnhanced<ILightPreprocess> data, NotificationContext notificationContext, SemaphoreSlim lockingThread = null, params object[] optional)
    {
        StartCoroutine(lightPreprocessDelegator.NotifyObserver(data, CELESTIAL_BODY_LIGHTNING_SUBJECT_UNIQUE_IDENTIFIER, notificationContext, lockingThread, optional));
    }

    public void OnNotifySubject(IObserverEnhanced<ILightPreprocess> data, NotificationContext notificationContext, params object[] optional)
    {
        StartCoroutine(lightPreprocessDelegator.NotifyObserver(data, this, notificationContext));
    }
}
