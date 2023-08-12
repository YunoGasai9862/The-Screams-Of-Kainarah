using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GlobalAccessAndGameHelper;
using System.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using System;

public class LightPoolObject : LightObserverPattern
{
    [Header("Insert Player Object")]
    [SerializeField] GameObject Player;

    public static Dictionary<GameObject, Candle> allCandlesInTheScene = new();
    public static List<GameObject> _allCandleObjects;
    private bool calculatingDistance = false;
    private float _screenWidth;
    private CancellationTokenSource tokenSource;

    private void Awake()
    {
        _allCandleObjects = GameObject.FindGameObjectsWithTag("candle").ToList();

        allCandlesInTheScene = fillupDictionaryWithCandleObjects(_allCandleObjects);

        _screenWidth = HelperFunctions.CalculateScreenWidth(Camera.main);

        tokenSource = new();

    }
    private async void Update()
    {
        if (!calculatingDistance)
        {
            await PlayersDistanceFromCandles(allCandlesInTheScene, _screenWidth, tokenSource.Token);
        }
    }


    private Dictionary<GameObject, Candle> fillupDictionaryWithCandleObjects(List<GameObject> array)
    {
        Dictionary<GameObject, Candle> _candleObjects = new();
        foreach (GameObject value in array)
        {
            Candle _temp = new(); //this fixed the issue!!!
            _temp.LightName = value.name;
            _temp.canFlicker = false;
            _candleObjects[value] = _temp;
            Debug.Log(value);
        }

        return _candleObjects;
    }

    private async Task PlayersDistanceFromCandles(Dictionary<GameObject, Candle> dict, float acceptedDistance, CancellationToken token)
    {
        calculatingDistance = true;
        foreach (GameObject value in dict.Keys) //allows modifying the copy of the keys, etc in the dictionary
        {
            Candle _candle = new();
            _candle.LightName = dict[value].LightName;
            _candle.canFlicker = false;

            if (Vector2.Distance(Player.transform.position, value.transform.position) < acceptedDistance)
            {
                _candle.canFlicker = true;
            }
            Debug.Log("Here");

            await NotifyAllLightObserversAsync(_candle);

            if (token.IsCancellationRequested)
            {
                Debug.Log("Cancelling");
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
