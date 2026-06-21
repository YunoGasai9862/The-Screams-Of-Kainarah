using Assets.Scripts.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager: Scene
{
    public static float ManipulateHealth (float currentHealth, float healAmount)
    {
        float newHealth = currentHealth + healAmount;
        return newHealth;
    }

}
