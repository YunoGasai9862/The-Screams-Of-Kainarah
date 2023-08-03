using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class candleFlickering : SubjectsToBeNotified
{
    private Light2D m_light;

    [Header("Light Intensity Swing Values")]
    public float maxIntensity;
    public float minIntensity;

    private void Awake()
    {
        m_light = GetComponent<Light2D>();

    }
    private void Start()
    {
        StartCoroutine(lightFlicker(maxIntensity, minIntensity));
    }

    private IEnumerator lightFlicker(float maxIntensity, float minIntensity)
    {
        float _lightFlickerValue = Random.Range(minIntensity, maxIntensity);
        m_light.intensity = _lightFlickerValue;
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(lightFlicker(maxIntensity, minIntensity));
    }

}
