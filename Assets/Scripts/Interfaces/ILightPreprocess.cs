using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public interface ILightPreprocess
{
    public abstract IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(LightPackage lightPackage, float delayBetweenExecution = 0);
}