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

    public LightEntity MoonLight { get; set; }
    private SemaphoreSlim _semaphoreSlim;


    private AsyncCoroutine AsyncCoroutineInstance { get; set; }

    private void Awake()
    {
        _semaphoreSlim = new SemaphoreSlim(semaPhoreSlimCount); //i already have one semaphoreSlim with 0 in another script, hence initializing it with 1

        StartCoroutine(AsyncCoroutineDelegator.NotifySubject(this));
    }

    private async Task CelestialBodyLightEffects(LightEntity entity, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim)
    {
        await semaphoreSlim.WaitAsync(); //waits for the thread to become available

        try
        {
            //update this somehow to run in a loop
            Debug.Log($"Moon Light: {MoonLight.ToString()}");

            //do it this way!!
            AsyncCoroutineInstance.ExecuteAsyncCoroutine(lightPreProcessWrapper.LightCustomPreprocess().GenerateCustomLighting());
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

    //please remove this and update it!!!
    public async void OnNotify(LightEntity data, NotificationContext notificationContext, params object[] optional)
    {
        MoonLight = new LightEntity(data.LightName, data.UseCustomTinkering, minInnerRadius, maxInnerRadius, minOuterRadius, maxOuterRadius);

        CancellationTokenSource source = new CancellationTokenSource();

        CancellationToken token = source.Token;

        await CelestialBodyLightEffects(MoonLight, token, _semaphoreSlim);
    }

    public void OnNotify(AsyncCoroutine data, NotificationContext notificationContext, params object[] optional)
    {
        AsyncCoroutineInstance = data;
    }
}
