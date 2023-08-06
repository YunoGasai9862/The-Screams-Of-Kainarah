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
    private Candle _temp = new();
    private float _screenWidth;

    private void Awake()
    {
        _allCandleObjects = GameObject.FindGameObjectsWithTag("candle").ToList();

        fillupDictionaryWithCandleObjects(_allCandleObjects);

        _screenWidth = HelperFunctions.CalculateScreenWidth(Camera.main);

    }
    private void Update()
    {
        if (!calculatingDistance)
            StartCoroutine(PlayersDistanceFromCandles(allCandlesInTheScene, _screenWidth));
    }

    private void fillupDictionaryWithCandleObjects(List<GameObject> array)
    {
        Candle _temp = new();
        foreach (GameObject value in array)
        {
            _temp.LightName = value.name;
            _temp.canFlicker = false;
            allCandlesInTheScene[value] = _temp;
        }
    }

    private IEnumerator PlayersDistanceFromCandles(Dictionary<GameObject, Candle> dict, float acceptedDistance)
    {
        calculatingDistance = true;

        foreach (GameObject value in dict.Keys.ToList()) //allows modifying the copy of the keys, etc in the dictionary
        {
            _temp = dict.GetValueOrDefault(value);

            if (Vector2.Distance(Player.transform.position, value.transform.position) < acceptedDistance)
            {
                _temp.canFlicker = true;
            }
            else
            {
                _temp.canFlicker = false;
            }

            dict[value] = _temp; //updates the value
            NotifyAllLightObservers(_temp);

        }
        calculatingDistance = false;

        yield return null;
    }

}
