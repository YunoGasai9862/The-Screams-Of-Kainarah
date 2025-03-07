using UnityEngine.Rendering.Universal;

public class LightPackage
{
    public Light2D LightSource { get; set; }

    public LightProperties LightProperties { get; set; }

    public ILightPreprocess LightPreprocess { get; set; }
}
