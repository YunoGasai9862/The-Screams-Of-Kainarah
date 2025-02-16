using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour, ILightPreprocess
{
    public async IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(Light2D light, float minIntensity, float maxIntensity, SemaphoreSlim couroutineBlocker, float minInnnerRadius, float maxInnerRadius, float minOuterRadius, float maxOuterRadius, float delayBetweenExecution)
    {
        while (true)
        {
            light.intensity = await GenerateLightRadia(minOuterRadius, maxOuterRadius);
            light.pointLightInnerRadius = await GenerateLightRadia(minInnnerRadius, maxInnerRadius);
            light.pointLightOuterRadius = await GenerateLightIntensityAsync(minIntensity, maxIntensity);
            //couroutineBlocker.Release();
            await Task.Delay(TimeSpan.FromSeconds(delayBetweenExecution));

            yield return null;
        }
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
