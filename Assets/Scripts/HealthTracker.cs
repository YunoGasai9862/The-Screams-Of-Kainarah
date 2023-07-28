using UnityEngine;

public class HealthTracker : MonoBehaviour
{
    [SerializeField] PlayerHelperClassForOtherPurposes _playerHelperClass;
    public float ReturnMainPlayerHealth()
    {
        return (float)_playerHelperClass.Health;

    }

    public float ReturnBossHealth()
    {
        // return (float)Movement.MAXHEALTH;
        return (float)BossScript.MAXHEALTH;
    }
}
