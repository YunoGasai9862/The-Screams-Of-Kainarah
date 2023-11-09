using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MoonMovement : MonoBehaviour
{
    [Header("Custom Variables")]
    [SerializeField] float moonSpeed;
    [SerializeField] float XOffset, YOffset, ZOffset;

    private SemaphoreSlim semaphoreSlim= new SemaphoreSlim(1);
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;

    private void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;

    }


    async void Update()
    {
        await moveToFollowPlayer(gameObject.transform, XOffset, YOffset, ZOffset, moonSpeed);   
    }

    private async Task<bool> moveToFollowPlayer(Transform targetToFollow, float xOffset, float yOffset, float zOffset, float speed)
    {

        await semaphoreSlim.WaitAsync();
        await Task.Delay(TimeSpan.FromSeconds(0f));
        if(!cancellationToken.IsCancellationRequested)
        {
            try
            {
                FollowPlayer.TrackPlayer(targetToFollow, xOffset, yOffset, zOffset, speed);

            }
            catch (OperationCanceledException ex)
            {
                Debug.LogException(ex);
                return false;
            }
            finally
            {
                semaphoreSlim.Release();

            }
        }

        return true;
    }

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
        semaphoreSlim.Release();
    }
}
