using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CelestialBodies : LightObserverPattern
{
    [SerializeField] CelestialBodyEvent celestialBodyEvent;

    private LightEntity m_light = new LightEntity();
    private CancellationTokenSource m_cancellationTokenSource;
    private CancellationToken m_cancellationToken;
    private SemaphoreSlim _semaphoreSlim;

    private void Awake()
    {
        m_cancellationTokenSource = new CancellationTokenSource();
        _semaphoreSlim = new SemaphoreSlim(1); //i already have one semaphoreSlim with 0 in another script, hence initializing it with 1
        m_cancellationToken = m_cancellationTokenSource.Token;
        celestialBodyEvent.AddListener(AddMoonLightProperties);
    }
    private async Task CelestialBodyLightEffects(LightEntity entity, CancellationToken cancellationToken)
    {
        await _semaphoreSlim.WaitAsync(); //waits for the thread to become available
        if (!m_cancellationTokenSource.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(.5));

            try
            {
                await NotifyAllLightObserversAsync(entity, cancellationToken);
            }
            catch (OperationCanceledException) //catches the exception, and gracefully exits
            {
                return;
            }
            finally
            {

                _semaphoreSlim.Release(); //it releases, allowing it to run again
            }
        }
    }
    private void OnDisable()
    {
        m_cancellationTokenSource.Cancel();
    }

    private async void AddMoonLightProperties(LightEntity lightEntity)
    {
        await CelestialBodyLightEffects(m_light, m_cancellationToken);
    }

}
