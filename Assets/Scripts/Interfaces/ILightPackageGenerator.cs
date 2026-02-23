using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
public interface ILightPackageGenerator
{
    public IEnumerator PingCustomLightning(LightPackage lightPackage, INotify<LightPackage> observer, float delayPerExecutionInSeconds = 1f);
}