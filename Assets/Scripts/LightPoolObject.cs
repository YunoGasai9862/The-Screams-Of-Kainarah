using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LightPoolObject : MonoBehaviour, ISubject<IObserver<LightEntity>>
{
    [Header("Insert Player Tag")]
    [SerializeField] string PlayerTag;

    [Header("Insert LightProcessor Delegator")]
    [SerializeField] LightProcessorDelegator lightProcessorDelegator;


    private float m_screenWidth;
    private CancellationTokenSource tokenSource;
    private GameObject m_player;

    private void Awake()
    {
        m_screenWidth = HelperFunctions.CalculateScreenWidth(Camera.main);

        tokenSource = new();

    }
    private void Start()
    {
        //use observer pattern maybe
        m_player = GameObject.FindWithTag(PlayerTag);

        lightProcessorDelegator.Subject.SetSubject(this);
    }

    private Task PingCustomLightning(IObserver<LightEntity> data, NotificationContext notificationContext, GameObject player, float acceptedDistance, CancellationToken token)
    {
        //LightEntity lightEntity = new LightEntity()
        //{
        //    LightName = notificationContext.GameObjectName,

        //    ShouldLightPulse = false
        //};

        //if (Vector2.Distance(player.transform.position, notificationContext.GameObject.transform.position) < acceptedDistance)
        //{
        //    lightEntity.ShouldLightPulse = true;
        //}

        try
        {
            StartCoroutine(lightProcessorDelegator.NotifyObserver(data, lightEntity, new NotificationContext()
            {
                GameObjectName = this.gameObject.name,
                GameObjectTag = this.gameObject.tag
            }));

        }
        catch (OperationCanceledException ex)
        {
            Debug.LogError($"Message: {ex.Message}");
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public async void OnNotifySubject(IObserver<LightEntity> data, NotificationContext notificationContext, params object[] optional)
    {
        await PingCustomLightning(data, notificationContext, m_player, m_screenWidth, tokenSource.Token);
    }

    private void OnDisable()
    {
        tokenSource.Cancel();
    }
}
