using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MoonShimmer : MonoBehaviour, ILightPreprocess
{
    [SerializeField] CelestialBodyEvent celestialBodyEvent;
    [SerializeField] Light2D moonLight;
    private void Update()
    {

    }
    public IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(Light2D light, float minIntensity, float maxIntensity, SemaphoreSlim couroutineBlocker, float minInnnerRadius, float maxInnerRadius, float minOuterRadius, float maxOuterRadius)
    {
        throw new System.NotImplementedException();
    }
    public Task<Light2D> ActivateContinuousShimmer(Light2D light, float time, float maxIntensity)
    {
        light.intensity = Mathf.PingPong(time, maxIntensity);
        return Task.FromResult(light);
    }
}
