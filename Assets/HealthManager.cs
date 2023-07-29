using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] List<string> _enemiesNames;
    private static HealthClass _mainPlayer;
    private static HealthClass _boss;
    private Dictionary<string, HealthClass> _allEnemies;

    private void Awake()
    {
        _mainPlayer = new HealthClass(100);
        _boss = new HealthClass(100);
    }

    public static float getPlayerHealth { get => _mainPlayer.EntityHealth; set => _mainPlayer.EntityHealth = value; }
    public static float getBossHealth { get => _boss.EntityHealth; set => _boss.EntityHealth = value; }

    public static void addAllPotentialEnemiesFromTheScene()
    {

    }


}
