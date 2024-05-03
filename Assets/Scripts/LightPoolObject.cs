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

    public static Dictionary<GameObject, LightEntity> allCandlesInTheScene = new();
    public static List<GameObject> _allCandleObjects;
    private bool calculatingDistance = false;
    private float _screenWidth;
    private CancellationTokenSource tokenSource;
    private GameObject _player;

    private void Awake()
    {
        _allCandleObjects = GameObject.FindGameObjectsWithTag("candle").ToList();

        allCandlesInTheScene = fillupDictionaryWithCandleObjects(_allCandleObjects);

        _screenWidth = HelperFunctions.CalculateScreenWidth(Camera.main);

        tokenSource = new();

    }
    private void Start()
    {
        _player = GameObject.FindWithTag(PlayerTag);
    }
    private async void Update()
    {
        if (!calculatingDistance)
        {
            await PlayersDistanceFromCandles(allCandlesInTheScene, _screenWidth, tokenSource.Token);
        }

    }

    private Dictionary<GameObject, LightEntity> fillupDictionaryWithCandleObjects(List<GameObject> array)
    {
        Dictionary<GameObject, LightEntity> _candleObjects = new();
        foreach (GameObject value in array)
        {
            LightEntity _temp = new(); //this fixed the issue!!!
            _temp.LightName = value.name;
            _temp.UseCustomTinkering = false;
            _candleObjects[value] = _temp;
        }

        return _candleObjects;
    }

    private async Task PlayersDistanceFromCandles(Dictionary<GameObject, LightEntity> dict, float acceptedDistance, CancellationToken token)
    {
        calculatingDistance = true;
        foreach (GameObject value in dict.Keys) //allows modifying the copy of the keys, etc in the dictionary
        {
            LightEntity _candle = new();
            _candle.LightName = dict[value].LightName;
            _candle.UseCustomTinkering = false;

            if (Vector2.Distance(_player.transform.position, value.transform.position) < acceptedDistance)
            {
                _candle.UseCustomTinkering = true;
            }

            try
            {
                await NotifyAllLightObserversAsync(_candle, token);

            }
            catch (OperationCanceledException) //works (making use of Exceptions)
            {
                calculatingDistance = false;
                return;
            }

        }
        calculatingDistance = false;

    }
    private void OnDisable()
    {
        tokenSource.Cancel();

    }

}
