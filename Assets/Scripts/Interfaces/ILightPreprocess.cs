using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public interface ILightPreprocess
{
    public abstract IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(Light2D light, LightEntity lightData, SemaphoreSlim couroutineBlocker, float delayBetweenExecution = 0);

}