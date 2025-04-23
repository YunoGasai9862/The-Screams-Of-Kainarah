using System.Collections;

public interface ILightPackageGenerator
{
    public IEnumerator PingCustomLightning(LightPackage lightPackage, IObserver<LightPackage> observer, float delayPerExecutionInSeconds = 1f);
}