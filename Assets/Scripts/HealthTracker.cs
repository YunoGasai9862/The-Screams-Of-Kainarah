using UnityEngine;

public class HealthTracker : MonoBehaviour
{
    public static float ReturnMainPlayerHealth()
    {
        return (float)PlayerHelperClassForOtherPurposes.MAXHEALTH;

    }

    public static float ReturnBossHealth()
    {
        // return (float)Movement.MAXHEALTH;
        return (float)BossScript.MAXHEALTH;
    }
}
