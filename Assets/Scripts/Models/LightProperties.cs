
using System;
using UnityEngine;

[Serializable]
public class LightProperties
{
    [SerializeField]
    private string m_LightName;
    [SerializeField]
    private bool m_ShouldLightPulse;
    [SerializeField]
    private float m_InnerRadiusMin;
    [SerializeField]
    private float m_InnerRadiusMax;
    [SerializeField]
    private float m_OuterRadiusMin;
    [SerializeField]
    private float m_OuterRadiusMax;
    [SerializeField]
    private float m_MaxLightIntensity;
    [SerializeField]
    private float m_MinLightIntensity;

    public string LightName { get => m_LightName; set => m_LightName = value; }
    public bool ShouldLightPulse { get => m_ShouldLightPulse; set => m_ShouldLightPulse = value; }
    public float InnerRadiusMin { get => m_InnerRadiusMin; set => m_InnerRadiusMin = value; }
    public float InnerRadiusMax { get => m_InnerRadiusMax; set => m_InnerRadiusMax = value; }
    public float OuterRadiusMin { get => m_OuterRadiusMin; set => m_OuterRadiusMin = value; }
    public float OuterRadiusMax { get => m_OuterRadiusMax; set => m_OuterRadiusMax = value; }
    public float MaxLightIntensity { get => m_MaxLightIntensity; set => m_MaxLightIntensity = value; }
    public float MinLightIntensity { get => m_MinLightIntensity; set => m_MinLightIntensity = value; }

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
}