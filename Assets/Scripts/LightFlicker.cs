using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LightFlicker : MonoBehaviour, ILightPreprocess, ISubjectNotifier<IObserver<LightFlicker, ILightPreprocess>>
{
    private const string LIGHT_FLICKER_SUBJECT_UNIQUE_IDENTIFIER = "light-flicker";

    [SerializeField]
    LightFlickerPreprocessDelegator lightFlickerPreprocessorDelegator;
    private void Start()
    {
        //create new entry
        lightFlickerPreprocessorDelegator.SubjectsDict.Add(LIGHT_FLICKER_SUBJECT_UNIQUE_IDENTIFIER, new SubjectNotifier<IObserver<LightFlicker, ILightPreprocess>>());
        //set subject
        lightFlickerPreprocessorDelegator.SubjectsDict[LIGHT_FLICKER_SUBJECT_UNIQUE_IDENTIFIER].SetSubject(this);
        //notify the observer with the key
        //maybe just broadcast it to all observers so far - add that mechanism in the child class maybe?
        StartCoroutine(lightFlickerPreprocessorDelegator.NotifyObserver(null, LIGHT_FLICKER_SUBJECT_UNIQUE_IDENTIFIER));
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
        StartCoroutine(lightFlickerPreprocessorDelegator.NotifyObserver(data, this));
    }

    public void NotifySubjectForActivation(NotificationContext notificationContext, params object[] optional)
    {
        //yayay
        throw new NotImplementedException();
    }
}
