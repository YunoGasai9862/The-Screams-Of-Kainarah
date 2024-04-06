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

    private void Awake()
    {
        uiParticleSystemEvent.AddListener(UpdateAlphaChannel);
        _uiParticleSystem = GetComponent<ParticleSystem>();
        //_psMaterial = GetComponent<Material>(); 
       
    }

    async void Start()
    {
        await SetAlphaValue(_psMaterial, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAlphaChannel(float value)
    {
        Debug.Log($"Particle UI System {value}");

    }

    private async Task SetAlphaValue(Material psMaterial, float value)
    {
        Color temp = new Color(psMaterial.color.r, psMaterial.color.g, psMaterial.color.b, value);
        psMaterial.color = temp;
        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

}
