using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AngelCircumvention : MonoBehaviour
{
    [Header("Custom Variables")]
    [SerializeField] float centerPoint;
    [SerializeField] float radius;

    private SemaphoreSlim semaphore;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;

    private float angle = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async Task CircumventAroundAPoint(float center, float radius)
    {

    }

    private async Task<float> checkAngle(float angle)
    {
        await Task.Delay(TimeSpan.FromSeconds(0));
        return angle > 360 ? 0 : angle;
    }
}
