using UnityEngine;

public class CelestialBodiesLightPackageGenerator : MonoBehaviour, IObserver<LightPackage>
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;

    public void OnNotify(LightPackage data, NotificationContext notificationContext, params object[] optional)
    {
        throw new System.NotImplementedException();
    }
}