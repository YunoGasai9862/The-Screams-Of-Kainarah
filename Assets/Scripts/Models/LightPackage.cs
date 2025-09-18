using System.Threading;
using UnityEngine.Rendering.Universal;

public class LightPackage
{
    public Light2D LightSource { get; set; }

    public LightProperties LightProperties { get; set; }

    public ILightPreprocess LightPreprocess { get; set; }

    public SemaphoreSlim LightSemaphore { get; set; }

    public CancellationToken CancellationToken { get; set; }

    public override string ToString()
    {
        return $"LightProperties: {LightProperties}, LightSource: {LightSource}, ILightPreprocess: {LightPreprocess}";
    }
}
