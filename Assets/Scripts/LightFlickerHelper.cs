using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlickerHelper : MonoBehaviour
{
    public static async IAsyncEnumerator<WaitForSeconds> lightFlicker(Light2D light, float minIntensity, float maxIntensity, SemaphoreSlim couroutineBlocker)
    {
        float _lightFlickerValue = await GenerateLightIntensityAsync(minIntensity, maxIntensity);
        light.intensity = _lightFlickerValue;
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
}
