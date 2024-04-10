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
    [SerializeField] Material _psMaterial;
    private ParticleSystem _uiParticleSystem;
    public float InitialAlphaValue { get; set; }
    public float NewAlphaValue { get; set; } = 0f;
    public AnimateProperty AnimatePropertyInstance {get; set;}
    private async void Awake()
    {
        await uiParticleSystemEvent.AddListener(UpdateAlphaChannel);
        _uiParticleSystem = GetComponent<ParticleSystem>();
        InitialAlphaValue = await GetMaterialsAlphaValue(_psMaterial);

        if(AnimatePropertyInstance == null)
            AnimatePropertyInstance = new AnimateProperty();
    }

    async void Start()
    {
        await SetAlphaValue(_psMaterial, 0);
    }
    public void UpdateAlphaChannel(float value)
    {
        Debug.Log($"Particle UI System {value}, newAlphaValue {NewAlphaValue}");
        NewAlphaValue = NewAlphaValue + (value / 255.0f); //convert it to a scale of 1 
        _= SetAlphaValue(_psMaterial, NewAlphaValue);
    }

    private async Task SetAlphaValue(Material psMaterial, float value)
    {
 
        Color temp = new Color(psMaterial.color.r, psMaterial.color.g, psMaterial.color.b, value);
        StartCoroutine(AnimatePropertyInstance.AnimColor(psMaterial.color, temp, 2f));
        psMaterial.SetColor("_BaseColor", temp);  //use it like tween
        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    private Task<float> GetMaterialsAlphaValue(Material psMaterial)
    {
        Color materialsColor = psMaterial.GetColor("_BaseColor");
        Debug.Log($"Initial Alpha Value: {materialsColor.a}");
        return Task.FromResult(materialsColor.a);
    }

    private async void OnDisable()
    {
       await SetAlphaValue(_psMaterial, InitialAlphaValue);
    }
}
