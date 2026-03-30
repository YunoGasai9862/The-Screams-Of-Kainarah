using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Subject(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(CelestialBodyLightning), ContextType = typeof(ILightPreprocess))]
public class CelestialBodyLightning : MonoBehaviour, ILightPreprocess, IRequest<ILightPreprocess>
{
    private Delegator Delegator { get; set; }

    private async void Start()
    {
        Delegator = await Helper.GetDelegator<Delegator>();
    }

    public async IAsyncEnumerator<WaitForSeconds> GenerateCustomLighting(LightPackage lightPackage, float delayBetweenExecution = 0)
    {
        lightPackage.LightSource.intensity = Mathf.PingPong(Time.time, lightPackage.LightProperties.MaxLightIntensity) + (lightPackage.LightProperties.MinLightIntensity);
        lightPackage.LightSource.pointLightOuterRadius = Mathf.PingPong(Time.time, lightPackage.LightProperties.OuterRadiusMax) + lightPackage.LightProperties.OuterRadiusMin;
        lightPackage.LightSource.pointLightInnerRadius = Mathf.PingPong(Time.time, lightPackage.LightProperties.InnerRadiusMax) + lightPackage.LightProperties.InnerRadiusMin;

        lightPackage.LightSemaphore.Release();
        yield return null;
    }

    public IEnumerator Request()
    {
        yield return StartCoroutine(Delegator.NotifyObservers(new SubjectContext<ILightPreprocess>() { Data =  this,  EntityType = typeof(CelestialBodyLightning) }, this));
    }
}
