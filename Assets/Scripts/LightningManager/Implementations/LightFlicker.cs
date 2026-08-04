using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.BaseScene;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(LightFlicker), ContextType = typeof(ILightPreprocess))]
public class LightFlicker : MonoBehaviorScene, Assets.Scripts.Interfaces.Mediator.EnhancedV1.IRequest<ILightPreprocess>, ILightPreprocess
{
    private Delegator Delegator { get; set; }

    private async void Start()
    {
        StartCoroutine((await (await GetBaseScene()).GetSceneUtilsAsync()).GetDelegator<Delegator>(value => Delegator = value));
    }

    public async IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(LightPackage lightPackage, float delayBetweenExecution = 0)
    {
        lightPackage.LightSource.intensity = lightPackage.LightProperties.ShouldLightPulse ?
            await GenerateLightIntensityAsync(lightPackage.LightProperties.MinLightIntensity, lightPackage.LightProperties.MaxLightIntensity) : lightPackage.LightSource.intensity;
        lightPackage.LightSource.pointLightInnerRadius = lightPackage.LightProperties.ShouldLightPulse?
            await GenerateLightRadia(lightPackage.LightProperties.InnerRadiusMin, lightPackage.LightProperties.InnerRadiusMax) : lightPackage.LightSource.pointLightInnerRadius;
        lightPackage.LightSource.pointLightOuterRadius = lightPackage.LightProperties.ShouldLightPulse ?
            await GenerateLightRadia(lightPackage.LightProperties.OuterRadiusMin, lightPackage.LightProperties.OuterRadiusMax) : lightPackage.LightSource.pointLightOuterRadius;

        lightPackage.LightSemaphore.Release();

        yield return null;
    }

    public Task<float> GenerateLightIntensityAsync(float minIntensity, float maxIntensity)
    {
        return Task.FromResult(Random.Range(minIntensity, maxIntensity));
    }
    public Task<float> GenerateLightRadia(float minRadia, float maxRadia)
    {
        return Task.FromResult(Random.Range(minRadia, maxRadia));
    }

    public IEnumerator Request()
    {
        yield return StartCoroutine(Delegator.NotifyObservers(new SubjectContext<ILightPreprocess>() { Data = this, EntityType = typeof(ILightPreprocess) }, this));
    }
}
