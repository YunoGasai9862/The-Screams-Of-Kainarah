using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class candleFlickering : MonoBehaviour, IObserver<Candle>
{
    private Light2D m_light;

    [Header("Light Intensity Swing Values")]
    public float maxIntensity;
    public float minIntensity;

    [Header("Light Outer Radius")]
    public float maxOuterRadius;
    public float minOuterRadius;

    [Header("Add the Subject which willl be responsible for notifying")]
    public LightObserverPattern _subject;

    private Candle m_Candle;
    private bool coroutingIsRunning;
    private void Awake()
    {
        m_light = GetComponent<Light2D>();

    }
    private void OnEnable()
    {
        _subject.AddObserver(this);
    }

    private void OnDisable()
    {
        _subject.RemoveObserver(this);
    }
    private void Start()
    {
        StartCoroutine(lightFlicker(minIntensity, maxIntensity));
    }

    private IEnumerator lightFlicker(float minIntensity, float maxIntensity)
    {
        coroutingIsRunning = true;
        float _lightFlickerValue = Random.Range(minIntensity, maxIntensity);
        m_light.intensity = _lightFlickerValue;
        coroutingIsRunning = false;
        yield return new WaitForSeconds(0.2f);
    }

    public void OnNotify(ref Candle Data)
    {
        m_Candle = Data;
        if (m_Candle != null && m_Candle.LightName == transform.parent.name && m_Candle.canFlicker)
        {
            if (!coroutingIsRunning)
                StartCoroutine(lightFlicker(minIntensity, maxIntensity));
        }
        else
        {
            StopAllCoroutines();
        }
    }
}
