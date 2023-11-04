
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlickering : MonoBehaviour, IObserverAsync<Candle>
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
    private SemaphoreSlim m_Semaphore;

    private void Awake()
    {
        m_light = GetComponent<Light2D>();
        
        if(!RunAsyncCoroutineWaitForSeconds.GetIsAttached)  //if its false
            RunAsyncCoroutineWaitForSeconds.AttachToGameObject(); //make sure this happens first
    }
    private void OnEnable()
    {
        _subject.AddObserver(this);
    }

    private void OnDisable()
    {
        _subject.RemoveObserver(this);
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
                m_Semaphore= new SemaphoreSlim(0);
    
                RunAsyncCoroutineWaitForSeconds.RunTheAsyncCoroutine(LightFlickerHelper.lightFlicker(m_light, minIntensity, maxIntensity, m_Semaphore) , _cancellationToken); //Async runner

                await m_Semaphore.WaitAsync(); //similar to using a bool variable, initializing it with 0. The thread becomes lock, and released in the helper class function

                //successfully was able to do it! (Async convesion)

                if (_cancellationToken.IsCancellationRequested)
                {
                    return;
                }

 
            }

            if (m_Candle.LightName == transform.parent.name && !m_Candle.canFlicker)
            {
                StopAllCoroutines(); //the fix!
            }
        }

    }
}
