using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CelestialBodies : MonoBehaviour, IObserver<LightEntity>
{
    [SerializeField]
    public float minOuterRadius, maxOuterRadius, minInnerRadius, maxInnerRadius;

    [SerializeField]
    public int semaPhoreSlimCount;

    [SerializeField]
    public LightEntityDelegator lightEntityDelegator;
    public LightEntity MoonLight { get; set; }
    private SemaphoreSlim _semaphoreSlim;

    private void Awake()
    {
        _semaphoreSlim = new SemaphoreSlim(semaPhoreSlimCount); //i already have one semaphoreSlim with 0 in another script, hence initializing it with 1

        StartCoroutine(lightEntityDelegator.NotifySubject(this));
    }

    private async Task CelestialBodyLightEffects(LightEntity entity, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim)
    {
        await semaphoreSlim.WaitAsync(); //waits for the thread to become available

        try
        {
            //update this somehow to run in a loop
            Debug.Log($"Moon Light: {MoonLight.ToString()}");

            await NotifyAllLightObserversAsync(entity, cancellationToken);
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

    public async void OnNotify(LightEntity data, params object[] optional)
    {
        MoonLight = new LightEntity(data.LightName, data.UseCustomTinkering, minInnerRadius, maxInnerRadius, minOuterRadius, maxOuterRadius);

        CancellationTokenSource source = new CancellationTokenSource();

        CancellationToken token = source.Token;

        await CelestialBodyLightEffects(MoonLight, token, _semaphoreSlim);
    }
}
