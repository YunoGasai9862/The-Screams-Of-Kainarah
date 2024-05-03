using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MoonShimmer : MonoBehaviour, ILightPreprocess
{
    [SerializeField] CelestialBodyEvent celestialBodyEvent;

    private LightEntity _moonLightData = new LightEntity();
    private void Awake()
    {
        _moonLightData.LightName = transform.parent.name;
        _moonLightData.UseCustomTinkering = true;

        celestialBodyEvent.GetInstance().Invoke(_moonLightData);
    }

    public async IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(Light2D light, float minIntensity, float maxIntensity, SemaphoreSlim couroutineBlocker, float minInnnerRadius, float maxInnerRadius, float minOuterRadius, float maxOuterRadius)
    {
        ActivateContinuousShimmer(light, Time.time, minIntensity, maxIntensity, minOuterRadius, maxOuterRadius);
        await Task.Delay(TimeSpan.FromMilliseconds(0));

        //release it here
        couroutineBlocker.Release();
        yield return new WaitForSeconds(0);
    }
    public void ActivateContinuousShimmer(Light2D light, float time, float minIntensity, float maxIntensity, float minOuterRadius, float maxOuterRadius)
    {
        Debug.Log((minOuterRadius, maxOuterRadius));
        light.intensity = Mathf.PingPong(time, maxIntensity) + minIntensity;

    }
}
