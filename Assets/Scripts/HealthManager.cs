using Assets.Scripts.BaseScene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager: MonoBehaviorScene
{
    public static float ManipulateHealth (float currentHealth, float healAmount)
    {
        float newHealth = currentHealth + healAmount;
        return newHealth;
    }

}
