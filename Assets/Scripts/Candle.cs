public class LightEntity
{
    const float MIN_INNER_RADIUS = 0;
    const float MAX_INNER_RADIUS = 0;
    const float MIN_OUTER_RADIUS = 4.5f;
    const float MAX_OUTER_RADIUS = 4.7f;
    const float LIGHT_INTENSITY = 1f;

    public string LightName;
    public bool useCustomTinkering;

    //light properties
    public float innerRadiusMin = MIN_INNER_RADIUS;
    public float innerRadiusMax = MAX_INNER_RADIUS;
    public float outerRadiusMin = MIN_OUTER_RADIUS;
    public float outerRadiusMax = MAX_OUTER_RADIUS;
    public float lightIntensity = LIGHT_INTENSITY;

}
