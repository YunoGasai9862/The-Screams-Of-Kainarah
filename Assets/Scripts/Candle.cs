public class LightEntity
{
    public string LightName { get; set; }
    public bool UseCustomTinkering { get; set; }
    //light properties
    public float InnerRadiusMin { get; set; }
    public float InnerRadiusMax { get; set; }
    public float OuterRadiusMin { get; set; }
    public float OuterRadiusMax { get; set; }
    public float LightIntensity { get; set; }

    public LightEntity(string lightName, bool useCustomTinkering, float innerRadiusMin, float innerRadiusMax, float outerRadiusMin, float outerRadiusMax, float lightIntensity)
    {
        LightName = lightName;
        UseCustomTinkering = useCustomTinkering;
        InnerRadiusMin = innerRadiusMin;
        InnerRadiusMax = innerRadiusMax;
        OuterRadiusMin = outerRadiusMin;
        OuterRadiusMax = outerRadiusMax;
        LightIntensity = lightIntensity;
    }

    //use lightning from the script itself
    public LightEntity(string lightName, bool useCustomTinkering, float innerRadiusMin, float innerRadiusMax, float outerRadiusMin, float outerRadiusMax)
    {
        LightName = lightName;
        UseCustomTinkering = useCustomTinkering;
        InnerRadiusMin = innerRadiusMin;
        InnerRadiusMax = innerRadiusMax;
        OuterRadiusMin = outerRadiusMin;
        OuterRadiusMax = outerRadiusMax;
    }

    public LightEntity()
    {
        InnerRadiusMin = LightEntityConstants.MIN_INNER_RADIUS;
        InnerRadiusMax = LightEntityConstants.MAX_INNER_RADIUS;
        OuterRadiusMin = LightEntityConstants.MIN_OUTER_RADIUS;
        OuterRadiusMax = LightEntityConstants.MAX_OUTER_RADIUS;
        LightIntensity = LightEntityConstants.DEFAULT_LIGHT_INTENSITY;
    }

    public override string ToString()
    {
        return $"LightName: {LightName}, UseCustomTinkering: {UseCustomTinkering}, InnerRMin: {InnerRadiusMin}, InnerRMax: {InnerRadiusMax}, OuterRMin: {OuterRadiusMin}, OuterRMax:" +
            $"{OuterRadiusMax}, LightIntensity: {LightIntensity}";
    }
}
public static class LightEntityConstants
{
    public const float MIN_INNER_RADIUS = 0;
    public const float MAX_INNER_RADIUS = 0;
    public const float MIN_OUTER_RADIUS = 4.5f;
    public const float MAX_OUTER_RADIUS = 4.7f;
    public const float DEFAULT_LIGHT_INTENSITY = 1f;
}