using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MoonFlickering : LightObserverPattern
{
    [SerializeField]
    public bool canFlicker;

    private LightEntity m_light= new LightEntity();
    private CancellationTokenSource m_cancellationTokenSource;
    private CancellationToken m_cancellationToken;
    private SemaphoreSlim _semaphoreSlim;
    private bool canProceed=true;
    private void Awake()
    {
        m_cancellationTokenSource = new CancellationTokenSource();
        _semaphoreSlim= new SemaphoreSlim(0);
        m_cancellationToken = m_cancellationTokenSource.Token;
        m_light.canFlicker = canFlicker;
        m_light.LightName = transform.parent.name; //always add transform.parent.name

    }
    async void Update()
    {
        if(canProceed)
         await MoonShimmer(m_light, m_cancellationToken);
    }

    private async Task MoonShimmer(LightEntity entity, CancellationToken cancellationToken)
    {
        canProceed = false;
        if (!m_cancellationTokenSource.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));

            try
            {
                await NotifyAllLightObserversAsync(entity, cancellationToken, _semaphoreSlim);
                canProceed=true;
            }
            catch (OperationCanceledException) //catches the exception, and gracefully exits
            {
                return;
            }
        }
    }
    private void OnDisable()
    {
        m_cancellationTokenSource.Cancel();
    }
}
