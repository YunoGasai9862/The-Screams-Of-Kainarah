public class LightProperties
{
    public string LightName { get; set; }
    public bool ShouldLightPulse { get; set; }
    public float InnerRadiusMin { get; set; }
    public float InnerRadiusMax { get; set; }
    public float OuterRadiusMin { get; set; }
    public float OuterRadiusMax { get; set; }
    public float MaxLightIntensity { get; set; }
    public float MinLightIntensity { get; set; }


    public LightProperties()
    {

    }

    public LightProperties(string lightName, bool shouldLightPulse, float innerRadiusMin, float innerRadiusMax, float outerRadiusMin, float outerRadiusMax, float maxlightIntensity, float minLightIntensity)
    {
        LightName = lightName;
        ShouldLightPulse = shouldLightPulse;
        InnerRadiusMin = innerRadiusMin;
        InnerRadiusMax = innerRadiusMax;
        OuterRadiusMin = outerRadiusMin;
        OuterRadiusMax = outerRadiusMax;
        MaxLightIntensity = maxlightIntensity;
        MinLightIntensity = minLightIntensity;
    }

    public override string ToString()
    {
        return $"LightName: {LightName}, ShouldLightPulse: {ShouldLightPulse}, InnerRMin: {InnerRadiusMin}, InnerRMax: {InnerRadiusMax}, OuterRMin: {OuterRadiusMin}, OuterRMax:" +
            $"{OuterRadiusMax}, MinLightIntensity: {MinLightIntensity} MaxLightIntensity: {MaxLightIntensity}";
    }

    public static LightProperties FromDefault(string lightName, bool shouldLightPulse)
    {
        return new LightProperties
        {
            LightName = lightName,
            ShouldLightPulse = shouldLightPulse,
            InnerRadiusMin = 1.0f,
            InnerRadiusMax = 2.0f,
            OuterRadiusMin = 3.0f,
            OuterRadiusMax = 5.0f,
            MaxLightIntensity = 0.5f,
            MinLightIntensity = 0.1f
        };
    }
}