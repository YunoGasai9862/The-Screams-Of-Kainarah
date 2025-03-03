using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour, ILightPreprocess
{

    public async IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(Light2D light, LightEntity lightData, SemaphoreSlim couroutineBlocker, float delayBetweenExecution = 0)
    {
        light.intensity = await GenerateLightRadia(lightData.OuterRadiusMin, lightData.OuterRadiusMax);
        light.pointLightInnerRadius = await GenerateLightRadia(lightData.InnerRadiusMin, lightData.InnerRadiusMax);
        light.pointLightOuterRadius = await GenerateLightIntensityAsync(lightData.MinLightIntensity, lightData.MaxLightIntensity);

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
}
