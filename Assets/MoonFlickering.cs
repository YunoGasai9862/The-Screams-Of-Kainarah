using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MoonFlickering : LightObserverPattern
{
    [SerializeField]
    public bool canFlicker;

    private LightEntity m_light;
    private CancellationTokenSource m_cancellationTokenSource;
    private CancellationToken m_cancellationToken;
    private void Awake()
    {
        m_cancellationTokenSource = new CancellationTokenSource();
        m_cancellationToken = m_cancellationTokenSource.Token;
        m_light.canFlicker = canFlicker;
        m_light.LightName = transform.name;

    }

    async void Start()
    {
       await NotifyAllLightObserversAsync(m_light, m_cancellationToken);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDisable()
    {
        m_cancellationTokenSource.Cancel();
    }
}
