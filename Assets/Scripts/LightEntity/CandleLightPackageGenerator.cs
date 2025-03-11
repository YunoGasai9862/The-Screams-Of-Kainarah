using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class CandleLightPackageGenerator : MonoBehaviour, IObserver<LightPackage>
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;

    private bool CalculateDistance()
    {
        return false;
    }

    private LightPackage PrepareLightPackage()
    {
        return null;
    }

    public void OnNotify(LightPackage data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        throw new System.NotImplementedException();
    }
}