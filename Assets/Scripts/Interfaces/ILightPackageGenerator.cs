using System.Collections;

public interface ILightPackageGenerator
{
    public IEnumerator PingCustomLightning(LightPackage lightPackage, INotify<LightPackage> observer, float delayPerExecutionInSeconds = 1f);
}