using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CelestialBodies : MonoBehaviour, IObserver<AsyncCoroutine>
{
    [SerializeField]
    public float minOuterRadius, maxOuterRadius, minInnerRadius, maxInnerRadius;

    [SerializeField]
    public int semaPhoreSlimCount;

    [SerializeField]
    public LightPreProcessWrapper lightPreProcessWrapper;

    [SerializeField]
    public AsyncCoroutineDelegator AsyncCoroutineDelegator;

    private LightEntity MoonLight { get; set; }

    private SemaphoreSlim SemaphoreSlim { get; set; }

    private CancellationTokenSource CancellationTokenSource { get; set; }
    private CancellationToken CancellationToken { get; set; }

    private async void Awake()
    {
        SemaphoreSlim = new SemaphoreSlim(semaPhoreSlimCount);

        MoonLight = await SetMoonLightData();

        CancellationTokenSource = new CancellationTokenSource();    

        CancellationToken = CancellationTokenSource.Token;

        StartCoroutine(AsyncCoroutineDelegator.NotifySubject(this));
    }

    private async Task CelestialBodyLightEffects(AsyncCoroutine asyncCoroutine, LightEntity entity, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim)
    {
        await semaphoreSlim.WaitAsync(); //waits for the thread to become available

        try
        {
            //update this somehow to run in a loop
            Debug.Log($"Moon Light: {MoonLight.ToString()}");

            //do it this way!!
            //asyncCoroutine.ExecuteAsyncCoroutine(lightPreProcessWrapper.LightCustomPreprocess().GenerateCustomLighting());
        }
        catch (OperationCanceledException) //catches the exception, and gracefully exits
        {
            return;
        }
        finally
        {
            semaphoreSlim.Release(); //it releases, allowing it to run again
        }
        
    }
    private Task<LightEntity> SetMoonLightData()
    {
        return Task.FromResult(new LightEntity()
        {
            LightName = transform.parent.name,
            UseCustomTinkering = true
        });
    }

    public async void OnNotify(AsyncCoroutine data, NotificationContext notificationContext, params object[] optional)
    {
        await CelestialBodyLightEffects(data, MoonLight, CancellationToken, SemaphoreSlim);
    }
}
