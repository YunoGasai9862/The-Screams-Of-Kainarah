using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTracker : MonoBehaviour
{
    public static float ReturnMainPlayerHealth()
    {
        return (float)Movement.MAXHEALTH;

    }

    public static float ReturnBossHealth()
    {
        // return (float)Movement.MAXHEALTH;
        return .0f;
    }
}
