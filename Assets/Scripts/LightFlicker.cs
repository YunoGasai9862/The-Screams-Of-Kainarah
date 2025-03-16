using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LightFlicker : MonoBehaviour, ILightPreprocess, ISubject<IObserver<LightFlicker, ILightPreprocess>>, ISubjectActivationNotifier<IObserver<LightFlicker, ILightPreprocess>>
{
    private const string LIGHT_FLICKER_SUBJECT_UNIQUE_IDENTIFIER = "light-flicker";

    LightPreprocessDelegator<LightFlicker> lightPreprocessDelegator;
    private void Start()
    {
        //create new entry
        lightPreprocessDelegator.SubjectsDict.Add(LIGHT_FLICKER_SUBJECT_UNIQUE_IDENTIFIER, new SubjectNotifier<IObserver<LightFlicker, ILightPreprocess>>());
        //set subject
        lightPreprocessDelegator.SubjectsDict[LIGHT_FLICKER_SUBJECT_UNIQUE_IDENTIFIER].SetSubject(this);

        //also set the subject for broadcast
        lightPreprocessDelegator.SubjectsDict[LIGHT_FLICKER_SUBJECT_UNIQUE_IDENTIFIER].SetSubjectActivationNotifier(this);
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

    public void OnNotifySubject(IObserver<LightFlicker, ILightPreprocess> data, NotificationContext notificationContext, params object[] optional)
    {
        StartCoroutine(lightPreprocessDelegator.NotifyObserver(data, this));
    }

    public void NotifySubjectOfActivation(IObserver<LightFlicker, ILightPreprocess> data, NotificationContext notificationContext, SemaphoreSlim lockingThread = null, params object[] optional)
    {
        StartCoroutine(lightPreprocessDelegator.NotifyObserver(data, LIGHT_FLICKER_SUBJECT_UNIQUE_IDENTIFIER, notificationContext, lockingThread, optional));
    }
}
