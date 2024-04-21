
using System;
using UnityEngine;

[Serializable]
public class LightPreProcessWrapper : MonoBehaviour
{
    [SerializeField]
    public ILightPreprocess LightPreprocess { get; set; }

    public LightPreProcessWrapper(ILightPreprocess lightPreprocess)
    {
        LightPreprocess = lightPreprocess;
    }
}