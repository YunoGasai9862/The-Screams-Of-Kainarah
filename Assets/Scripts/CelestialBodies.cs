using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CelestialBodies : LightObserverPattern
{
    [SerializeField] CelestialBodyEvent celestialBodyEvent;
    [SerializeField]
    public float minOuterRadius, maxOuterRadius, minInnerRadius, maxInnerRadius;

    public LightEntity MoonLight { get; set; }
    private CancellationTokenSource m_cancellationTokenSource;
    private CancellationToken m_cancellationToken;
    private SemaphoreSlim _semaphoreSlim;

    private void Awake()
    {
        m_cancellationTokenSource = new CancellationTokenSource();
        _semaphoreSlim = new SemaphoreSlim(1); //i already have one semaphoreSlim with 0 in another script, hence initializing it with 1
        m_cancellationToken = m_cancellationTokenSource.Token;

        celestialBodyEvent.AddListener(AddLightProperties);
    }


    private async void Update()
    {
        await CelestialBodyLightEffects(MoonLight, m_cancellationToken);
    }
    private async Task CelestialBodyLightEffects(LightEntity entity, CancellationToken cancellationToken)
    {
        await _semaphoreSlim.WaitAsync(); //waits for the thread to become available
        if (!m_cancellationTokenSource.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.1));

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
    private void AddLightProperties(LightEntity lightEntity)
    {
        MoonLight = new LightEntity(lightEntity.LightName, lightEntity.UseCustomTinkering, minInnerRadius, maxInnerRadius, minOuterRadius, maxOuterRadius);
    }
}
