using UnityEngine;

public class CelestialBodiesLightPackageGenerator : MonoBehaviour, IObserver<LightPackage>, IObserver<LightFlicker, ILightPreprocess>
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;
    LightFlickerPreprocessDelegator lightFlickerPreprocessDelegator;

    private void Start()
    {
        StartCoroutine(lightFlickerPreprocessDelegator.NotifySubject(this));
    }

    public void OnNotify(LightPackage data, NotificationContext notificationContext, params object[] optional)
    {
        throw new System.NotImplementedException();
    }

    void IObserver<LightFlicker, ILightPreprocess>.OnNotify(ILightPreprocess data, NotificationContext context, params object[] optional)
    {
        throw new System.NotImplementedException();
    }
}