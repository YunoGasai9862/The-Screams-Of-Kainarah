using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class LightPoolObject : MonoBehaviour
{
    public static Dictionary<GameObject, Candle> allCandlesInTheScene = new();
    public static List<GameObject> _allCandleObjects;

    private void Awake()
    {
        _allCandleObjects = GameObject.FindGameObjectsWithTag("candle").ToList();

        fillupDictionaryWithCandleObjects(_allCandleObjects);
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
}
