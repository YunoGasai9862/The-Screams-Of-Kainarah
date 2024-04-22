using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour, ILightPreprocess
{
    public async IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(Light2D light, float minIntensity, float maxIntensity, SemaphoreSlim couroutineBlocker, float minInnnerRadius, float maxInnerRadius, float minOuterRadius, float maxOuterRadius)
    {
        float _lightFlickerValue = await GenerateLightIntensityAsync(minIntensity, maxIntensity);
        float _lightInnerRadius = await GenerateLightRadia(minInnnerRadius, maxInnerRadius);
        float _lightOuterRadius = await GenerateLightRadia(minOuterRadius, maxOuterRadius);
        light.intensity = _lightFlickerValue;
        light.pointLightInnerRadius = _lightInnerRadius;
        light.pointLightOuterRadius = _lightOuterRadius;
        await Task.Delay(System.TimeSpan.FromSeconds(.2f));
        couroutineBlocker.Release();
        yield return new WaitForSeconds(.2f);
    }

    public Task<float> GenerateLightIntensityAsync(float minIntensity, float maxIntensity)
    {
        float intensity = Random.Range(minIntensity, maxIntensity);
        //add checks as well
        return Task.FromResult(intensity);
    }
    public Task<float> GenerateLightRadia(float minRadia, float maxRadia)
    {
        float radius = Random.Range(minRadia, maxRadia);
        return Task.FromResult(radius);

    }

    public Task<Light2D> ActivateContinuousShimmer(Light2D light, float time, float maxIntensity)
    {
        light.intensity = Mathf.PingPong(time, maxIntensity);
        return Task.FromResult(light);
    }
}
