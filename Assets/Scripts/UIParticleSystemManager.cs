using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParticleSystemManager : MonoBehaviour
{
    [SerializeField] ParticleSystemForceField particleSystemForceField; //play with it (customize it)
    [SerializeField] UIParticleSystemEvent uiParticleSystemEvent;

    private void Awake()
    {
        uiParticleSystemEvent.AddListener(UpdateAlphaChannel);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAlphaChannel(float value)
    {
        Debug.Log($"Particle UI System {value}");
    }

}
