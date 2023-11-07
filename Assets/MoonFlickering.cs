using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MoonFlickering : LightObserverPattern
{
    [SerializeField]
    public bool canFlicker;
    public float innerRadiusMin;
    public float innerRadiusMax;
    public float outerRadiusMin;
    public float outerRadiusMax;

    private LightEntity m_light= new LightEntity();
    private CancellationTokenSource m_cancellationTokenSource;
    private CancellationToken m_cancellationToken;
    private SemaphoreSlim _semaphoreSlim;
    private void Awake()
    {
        m_cancellationTokenSource = new CancellationTokenSource();
        _semaphoreSlim= new SemaphoreSlim(1); //i already have one semaphoreSlim with 0 in another script, hence initializing it with 1
        m_cancellationToken = m_cancellationTokenSource.Token;
        m_light.canFlicker = canFlicker;
        m_light.LightName = transform.parent.name; //always add transform.parent.name

    }
    async void Update()
    {
         await MoonShimmer(m_light, m_cancellationToken, innerRadiusMin, innerRadiusMax, outerRadiusMin, outerRadiusMax);
    }

    private async Task<float> GenerateRadia(float min, float max)
    {
        await Task.Delay(TimeSpan.FromSeconds(0.5));
        return UnityEngine.Random.Range(min, max);
    }

    private async Task MoonShimmer(LightEntity entity, CancellationToken cancellationToken, float innerMinRadius, float innerMaxRadius, float outerMinRadius, float outerMaxRadius)
    {
        await _semaphoreSlim.WaitAsync(); //waits for the thread to become available
        if (!m_cancellationTokenSource.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(.5));

            try
            {
                await GenerateRadia(innerMinRadius, innerMaxRadius);
                await GenerateRadia(outerMinRadius, outerMaxRadius);
                await NotifyAllLightObserversAsync(entity, cancellationToken);
            }
            catch (OperationCanceledException) //catches the exception, and gracefully exits
            {
                return;
            }
            finally {

                _semaphoreSlim.Release(); //it releases, allowing it to run again
            }
        }
    }
    private void OnDisable()
    {
        m_cancellationTokenSource.Cancel();
    }
}
