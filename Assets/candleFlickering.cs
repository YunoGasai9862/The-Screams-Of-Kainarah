using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class candleFlickering : MonoBehaviour, IObserverAsync<Candle>
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
    private bool coroutingIsRunning = false;

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
        //StartCoroutine(lightFlicker(minIntensity, maxIntensity));
    }

    private async IAsyncEnumerator<WaitForSeconds> lightFlicker(float minIntensity, float maxIntensity)
    {
        float _lightFlickerValue = await GenerateLightIntensityAsync(minIntensity, maxIntensity);
        m_light.intensity = _lightFlickerValue;
        yield return new WaitForSeconds(.2f);
        coroutingIsRunning = false;

    }
    private Task<float> GenerateLightIntensityAsync(float minIntensity, float maxIntensity)
    {
        float intensity = Random.Range(minIntensity, maxIntensity);
        //add checks as well
        return Task.FromResult(intensity);
    }

    public async Task OnNotify(Candle Data, CancellationToken _cancellationToken)
    {
        m_Candle = Data;

        Debug.Log(_cancellationToken.IsCancellationRequested);

        if (m_Candle != null)
        {

            if (m_Candle.LightName == transform.parent.name && m_Candle.canFlicker)
            {
                if (!coroutingIsRunning)
                {
                    coroutingIsRunning = true;

                    IAsyncEnumerator<WaitForSeconds> lightFlickerWaiting = lightFlicker(minIntensity, maxIntensity);

                    // StartCoroutine(lightFlicker(minIntensity, maxIntensity));
                    // IAsyncEnumer


                    if (_cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }


                }
            }

            if (m_Candle.LightName == transform.parent.name && !m_Candle.canFlicker)
            {
                StopAllCoroutines(); //the fix!
            }
        }

    }
}
