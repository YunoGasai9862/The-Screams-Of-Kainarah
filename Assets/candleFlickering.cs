
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
    private async IAsyncEnumerator<WaitForSeconds> lightFlicker(float minIntensity, float maxIntensity)
    {
        float _lightFlickerValue = await GenerateLightIntensityAsync(minIntensity, maxIntensity);
        m_light.intensity = _lightFlickerValue;
        await Task.Delay(System.TimeSpan.FromSeconds(.2f));
        coroutingIsRunning = false;
        yield return new WaitForSeconds(.2f);


    }
    private Task<float> GenerateLightIntensityAsync(float minIntensity, float maxIntensity)
    {
        float intensity = Random.Range(minIntensity, maxIntensity);
        //add checks as well
        return Task.FromResult(intensity);
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task OnNotify(Candle Data, CancellationToken _cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        m_Candle = Data;

        if (m_Candle != null)
        {

            if (m_Candle.LightName == transform.parent.name && m_Candle.canFlicker)
            {
                if (!coroutingIsRunning)
                {
                    coroutingIsRunning = true;

                    RunAsyncCoroutine<WaitForSeconds>.RunTheAsyncCoroutine(lightFlicker(minIntensity, maxIntensity) , _cancellationToken); //Async runner

                    //successfully was able to do it! (Async convesion)

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
