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
    public Color OriginalColor { get; set; }
    public float NewAlphaValue { get; set; } = 0f;
    public AnimatePropertyColor AnimatePropertyColorInstance {get; set;}
    public Material PSMaterial { get; set;}
    private async void Awake()
    {
        await uiParticleSystemEvent.AddListener(UpdateAlphaChannel);
        _uiParticleSystem = GetComponent<ParticleSystem>();
        PSMaterial = psMaterial;
        OriginalColor = await GetMaterialsOriginalColor(PSMaterial);

        if(AnimatePropertyColorInstance == null)
            AnimatePropertyColorInstance = new AnimatePropertyColor(ColorListener);
    }

    async void Start()
    {
        await SetAlphaValueForParticles(PSMaterial, 0, 2f);
    }
    public void UpdateAlphaChannel(float value)
    {
        Debug.Log($"Particle UI System {value}, newAlphaValue {NewAlphaValue}");
        NewAlphaValue = NewAlphaValue + (value / 255.0f); //convert it to a scale of 1 
        _= SetAlphaValueForParticles(PSMaterial, NewAlphaValue, 5f);
    }

    private async Task SetAlphaValueForParticles(Material psMaterial, float value, float duration)
    {
 
        Color temp = new Color(psMaterial.color.r, psMaterial.color.g, psMaterial.color.b, value);
        StartCoroutine(AnimatePropertyColorInstance.AnimColor(psMaterial.color, temp, duration));
        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    private Task<Color> GetMaterialsOriginalColor(Material psMaterial)
    {
        Color materialsColor = psMaterial.GetColor("_BaseColor");
        return Task.FromResult(materialsColor);
    }

    private void ColorListener(Color modifiedColor)
    {
        //set the material value here
        Debug.Log($"Modified Color: {modifiedColor}");
        PSMaterial.SetColor("_BaseColor", modifiedColor);  //use it like tween
    }

    private Task<Color> ResetColor()
    {
        return Task.FromResult(new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, OriginalColor.a));
    }

    private async void OnDisable()
    {
        PSMaterial.SetColor("_BaseColor", await ResetColor());
    }
}
