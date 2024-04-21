using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public interface ILightPreprocess
{
    public abstract IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(Light2D light, float minIntensity, float maxIntensity, SemaphoreSlim couroutineBlocker, float minInnnerRadius, float maxInnerRadius, float minOuterRadius, float maxOuterRadius);
}