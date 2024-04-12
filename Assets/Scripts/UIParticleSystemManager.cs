using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UIParticleSystemManager : MonoBehaviour
{
    [SerializeField] ParticleSystemForceField particleSystemForceField; //play with it (customize it)
    [SerializeField] UIParticleSystemEvent uiParticleSystemEvent;
    [SerializeField] Material psMaterial;
    private ParticleSystem _uiParticleSystem;
    public float InitialAlphaValue { get; set; }
    public float NewAlphaValue { get; set; } = 0f;
    public AnimatePropertyColor AnimatePropertyColorInstance {get; set;}
    public Material PSMaterial { get; set;}
    private async void Awake()
    {
        await uiParticleSystemEvent.AddListener(UpdateAlphaChannel);
        _uiParticleSystem = GetComponent<ParticleSystem>();
        PSMaterial = psMaterial;
        InitialAlphaValue = await GetMaterialsAlphaValue(PSMaterial);

        if(AnimatePropertyColorInstance == null)
            AnimatePropertyColorInstance = new AnimatePropertyColor(ColorListener);
    }

    async void Start()
    {
        await SetAlphaValue(PSMaterial, 0);
    }
    public void UpdateAlphaChannel(float value)
    {
        Debug.Log($"Particle UI System {value}, newAlphaValue {NewAlphaValue}");
        NewAlphaValue = NewAlphaValue + (value / 255.0f); //convert it to a scale of 1 
        _= SetAlphaValue(PSMaterial, NewAlphaValue);
    }

    private async Task SetAlphaValue(Material psMaterial, float value)
    {
 
        Color temp = new Color(psMaterial.color.r, psMaterial.color.g, psMaterial.color.b, value);
        StartCoroutine(AnimatePropertyColorInstance.AnimColor(psMaterial.color, temp, 2f));
        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    private Task<float> GetMaterialsAlphaValue(Material psMaterial)
    {
        Color materialsColor = psMaterial.GetColor("_BaseColor");
        Debug.Log($"Initial Alpha Value: {materialsColor.a}");
        return Task.FromResult(materialsColor.a);
    }

    private void ColorListener(Color modifiedColor)
    {
        //set the material value here
        PSMaterial.SetColor("_BaseColor", modifiedColor);  //use it like tween
    }

    private async void OnDisable()
    {
        if (isActiveAndEnabled)
        {
             await SetAlphaValue(PSMaterial, InitialAlphaValue);
        }
    }
}
