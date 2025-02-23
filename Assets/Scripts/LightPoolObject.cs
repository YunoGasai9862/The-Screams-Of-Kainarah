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
    }
    private async void Update()
    {
        //if (!calculatingDistance)
        //{
        //    await PlayersDistanceFromCandles(lightEntitiesDict, m_player, m_screenWidth, tokenSource.Token);
        //}
    }

    private Dictionary<GameObject, LightEntity> FillUpLightEntities(List<GameObject> candleObjects)
    {
        Dictionary<GameObject, LightEntity> candles = new Dictionary<GameObject, LightEntity>();

        foreach (GameObject candle in candleObjects)
        {
            candles[candle] = new LightEntity()
            {
                LightName = candle.name,
                UseCustomTinkering = false
            };
        }

        return candles;
    }

    private async Task PrepareDataAndPingCustomLightProcessing(IObserver<LightEntity> data, NotificationContext notificationContext, GameObject player, float acceptedDistance, CancellationToken token)
    {
        LightEntity lightEntity = new LightEntity()
        {
            LightName = notificationContext.GameObjectName,

            UseCustomTinkering = false
        };

        if (Vector2.Distance(player.transform.position, notificationContext.GameObject.transform.position) < acceptedDistance)
        {
            lightEntity.UseCustomTinkering = true;
        }

        try
        {
            //now revise this logic - and see how to make it less flicker - or only send a new notification for flicker once the user distance has passed away, dont do the same thing again and again!!

            
            //please use a delegator here, and not OnNotify!!!
            data.OnNotify(lightEntity, new NotificationContext()
            {
                GameObjectName = this.gameObject.name,
                GameObjectTag = this.gameObject.tag
            });

        }
        catch (OperationCanceledException)
        {
            return;
        }
    }

    public async void OnNotifySubject(IObserver<LightEntity> data, NotificationContext notificationContext, params object[] optional)
    {
        await PrepareDataAndPingCustomLightProcessing(data, notificationContext, m_player, m_screenWidth, tokenSource.Token);
    }

    private void OnDisable()
    {
        tokenSource.Cancel();
    }
}
