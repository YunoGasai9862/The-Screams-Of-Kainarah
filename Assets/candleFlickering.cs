using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEditor.Progress;

public class candleFlickering : MonoBehaviour
{
    private Light2D m_light;

    [Header("Light Intensity Swing Values")]
    public float maxIntensity;
    public float minIntensity;
    public float incrementor;

    private float m_intensityOffset = 0.01f;
    private bool iscycleFinished = false;

    private void Awake()
    {
        m_light = GetComponent<Light2D>();

    }

    // Update is called once per frame
    void Update()
    {

    }


    private bool flicker(float maxIntensity, float minIntensity)
    {
        float _temp = maxIntensity - m_intensityOffset;
        while (_temp < maxIntensity && _temp > minIntensity)
        {
            _temp -= incrementor * Time.deltaTime;
            m_light.intensity = _temp;

        }

        _temp = minIntensity + m_intensityOffset;
        while (_temp > minIntensity && _temp < maxIntensity)
        {
            _temp += incrementor * Time.deltaTime;
            m_light.intensity = _temp;

        }

        iscycleFinished = true;
        return iscycleFinished;

    }


}
