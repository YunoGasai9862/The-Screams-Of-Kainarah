
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public class LightPreProcessWrapper : MonoBehaviour
{
    [SerializeField]
    public MonoBehaviour LightPreprocess;

    public ILightPreprocess CastToILightPreprocess()
    {
        return LightPreprocess.GetType() == typeof(ILightPreprocess) ? LightPreprocess as ILightPreprocess : throw new ApplicationException("Processor should implement ILightProcess"); 
    }

    public Light2D GetLight2D()
    {
        return LightPreprocess.GetComponent<Light2D>() ?? throw new ApplicationException("Light 2D doesn't exist!");
    }
}