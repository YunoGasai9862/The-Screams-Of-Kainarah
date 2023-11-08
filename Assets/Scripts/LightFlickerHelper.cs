using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlickerHelper : MonoBehaviour
{
    const float MININNERRADIUS = 0; //default values
    const float MAXINNERRADIUS = 0;
    const float MINOUTERRADIUS = 4;
    const float MAXOUTERRADIUS = 5;

    public static async IAsyncEnumerator<WaitForSeconds> lightFlicker(Light2D light, float minIntensity, float maxIntensity, SemaphoreSlim couroutineBlocker, float minInnnerRadius= MININNERRADIUS, float maxInnerRadius= MAXINNERRADIUS, float minOuterRadius= MINOUTERRADIUS, float maxOuterRadius= MAXOUTERRADIUS)
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
    public static Task<float> GenerateLightIntensityAsync(float minIntensity, float maxIntensity)
    {
        float intensity = Random.Range(minIntensity, maxIntensity);
        //add checks as well
        return Task.FromResult(intensity);
    }
    public static Task<float> GenerateLightRadia(float minRadia, float maxRadia)
    {
        float radius = Random.Range(minRadia, maxRadia);
        return Task.FromResult(radius);

    }
}
