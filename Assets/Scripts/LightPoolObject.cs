using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LightPoolObject : LightObserverPattern
{
    [Header("Insert Player Tag")]
    [SerializeField] string PlayerTag;

    public static Dictionary<GameObject, LightEntity> lightEntitiesDict = new Dictionary<GameObject, LightEntity>();
    public static List<GameObject> candleObjects;
    private bool calculatingDistance = false;
    private float m_screenWidth;
    private CancellationTokenSource tokenSource;
    private GameObject m_player;

    private void Awake()
    {
        candleObjects = GameObject.FindGameObjectsWithTag("candle").ToList();

        lightEntitiesDict = FillUpLightEntities(candleObjects);

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
        if (!calculatingDistance)
        {
            await PlayersDistanceFromCandles(lightEntitiesDict, m_player, m_screenWidth, tokenSource.Token);
        }

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

    private async Task PlayersDistanceFromCandles(Dictionary<GameObject, LightEntity> lightEntities, GameObject player, float acceptedDistance, CancellationToken token)
    {
        calculatingDistance = true;

        foreach (GameObject lightEntity in lightEntities.Keys)
        {
            if (Vector2.Distance(player.transform.position, lightEntity.transform.position) < acceptedDistance)
            {
                lightEntities[lightEntity].UseCustomTinkering = true;
            }

            try
            {
                //now revise this logic - and see how to make it less flicker - or only send a new notification for flicker once the user distance has passed away, dont do the same thing again and again!!
                await NotifyAllLightObserversAsync(lightEntities[lightEntity], token);

            }
            catch (OperationCanceledException) //works (making use of Exceptions)
            {
                calculatingDistance = false;
                return;
            }

        }
        calculatingDistance = false;

    }

    private async Task CalculatesPlayerDistanceFromLightSource()
    {

    }

    private void OnDisable()
    {
        tokenSource.Cancel();
    }

}
