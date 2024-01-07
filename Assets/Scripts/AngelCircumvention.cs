using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AngelCircumvention : MonoBehaviour
{
    [Header("Custom Variables")]
    [SerializeField] Vector2 centerPoint;
    [SerializeField] double radius;

    private SemaphoreSlim semaphore;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;

    private double angle = 0;

    private void Start()
    {
        semaphore = new SemaphoreSlim(1);
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
    }

    // Update is called once per frame
    async void Update()
    {
        await semaphore.WaitAsync();

        if(!cancellationToken.IsCancellationRequested)
        {
            angle += Time.deltaTime;
            transform.position = await CircumventAroundAPoint(angle, centerPoint, radius, semaphore);
        }
    }

    private async Task<Vector2> CircumventAroundAPoint(double angle, Vector2 center, double radius, SemaphoreSlim semaphore)
    {
        //rcosQ and rsinQ
        double angleTemp = await checkAngle(angle);
        Vector2 position = new Vector2((float)(center.x + radius * Math.Cos(angleTemp)), (float)(center.y + radius * Math.Sin(angleTemp)));
        await Task.Delay(TimeSpan.FromSeconds(0));
        semaphore.Release();
        return position;
    }

    private async Task<double> checkAngle(double angle)
    {
        await Task.Delay(TimeSpan.FromSeconds(0));
        return angle > 360 ? 0 : angle;
    }

    private void OnDisable()
    {
        transform.position = centerPoint;
        cancellationTokenSource.Cancel();
    }
}
