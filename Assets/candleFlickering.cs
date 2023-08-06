using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class candleFlickering : MonoBehaviour, IObserver<Candle>
{
    private Light2D m_light;

    [Header("Light Intensity Swing Values")]
    public float maxIntensity;
    public float minIntensity;

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
        StartCoroutine(lightFlicker(maxIntensity, minIntensity));
    }

    private IEnumerator lightFlicker(float maxIntensity, float minIntensity)
    {
        coroutingIsRunning = true;
        float _lightFlickerValue = Random.Range(minIntensity, maxIntensity);
        m_light.intensity = _lightFlickerValue;
        yield return new WaitForSeconds(0.2f);
        coroutingIsRunning = false;
    }

    public void OnNotify(ref Candle Data)
    {
        m_Candle = Data;

        if (m_Candle != null && m_Candle.LightName == transform.parent.name && m_Candle.canFlicker)
        {
            if (!coroutingIsRunning)
                StartCoroutine(lightFlicker(maxIntensity, minIntensity));

        }
        else
        {
            StopAllCoroutines();
        }
    }
}
