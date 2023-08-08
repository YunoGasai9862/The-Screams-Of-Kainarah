using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GlobalAccessAndGameHelper;

public class LightPoolObject : LightObserverPattern
{
    [Header("Insert Player Object")]
    [SerializeField] GameObject Player;

    public static Dictionary<GameObject, Candle> allCandlesInTheScene = new();
    public static List<GameObject> _allCandleObjects;
    private bool calculatingDistance = false;
    private float _screenWidth;

    private void Awake()
    {
        _allCandleObjects = GameObject.FindGameObjectsWithTag("candle").ToList();

        allCandlesInTheScene = fillupDictionaryWithCandleObjects(_allCandleObjects);

        _screenWidth = HelperFunctions.CalculateScreenWidth(Camera.main);

    }
    private void Update()
    {
        if (!calculatingDistance)
            StartCoroutine(PlayersDistanceFromCandles(allCandlesInTheScene, _screenWidth));
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
        }

        return _candleObjects;
    }

    private IEnumerator PlayersDistanceFromCandles(Dictionary<GameObject, Candle> dict, float acceptedDistance)
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

            NotifyAllLightObservers(_candle);

        }
        calculatingDistance = false;

        yield return null;
    }

}
