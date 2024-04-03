using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParticleSystemManager : MonoBehaviour
{
    [SerializeField] ParticleSystemForceField particleSystemForceField;

    private UIParticleSystemEvent _uiParticleSystemEvent = new UIParticleSystemEvent();

    private void Awake()
    {
        _uiParticleSystemEvent.AddListener(UpdateAlphaChannel);
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

    }

    public UIParticleSystemEvent GetUIParticleSystemEventInstance()
    {
        return _uiParticleSystemEvent;
    }
}
