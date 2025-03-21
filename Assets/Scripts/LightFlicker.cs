using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LightFlicker : MonoBehaviour, ILightPreprocess, ISubject<IObserver<LightFlicker, ILightPreprocess>>, ISubjectActivationNotifier<IObserver<LightFlicker, ILightPreprocess>>
{
    private const string LIGHT_FLICKER_SUBJECT_UNIQUE_IDENTIFIER = "light-flicker";

    [SerializeField]
    LightPreprocessDelegatorManager lightPreprocessDelegatorManager;
    private void Start()
    {
        //create new entry
        lightPreprocessDelegatorManager.LightPreprocessDelegator.SubjectsDict.Add(LIGHT_FLICKER_SUBJECT_UNIQUE_IDENTIFIER, new SubjectNotifier<IObserver<MonoBehaviour, ILightPreprocess>>());
        //set subject
        lightPreprocessDelegatorManager.LightPreprocessDelegator.SubjectsDict[LIGHT_FLICKER_SUBJECT_UNIQUE_IDENTIFIER].SetSubject(this);

        //also set the subject for broadcast
        lightPreprocessDelegatorManager.LightPreprocessDelegator.SubjectsDict[LIGHT_FLICKER_SUBJECT_UNIQUE_IDENTIFIER].SetSubjectActivationNotifier(this);
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

    //test all casts please
    public void OnNotifySubject(IObserver<LightFlicker, ILightPreprocess> data, NotificationContext notificationContext, params object[] optional)
    {
        StartCoroutine(lightPreprocessDelegatorManager.LightPreprocessDelegator.NotifyObserver((IObserver<MonoBehaviour, ILightPreprocess>)data, this));
    }

    public void NotifySubjectOfActivation(IObserver<LightFlicker, ILightPreprocess> data, NotificationContext notificationContext, SemaphoreSlim lockingThread = null, params object[] optional)
    {
        StartCoroutine(lightPreprocessDelegatorManager.LightPreprocessDelegator.NotifyObserver((IObserver<MonoBehaviour, ILightPreprocess>)data, LIGHT_FLICKER_SUBJECT_UNIQUE_IDENTIFIER, notificationContext, lockingThread, optional));
    }
}
