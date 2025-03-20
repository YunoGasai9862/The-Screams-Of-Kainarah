using System.Threading;
using UnityEngine;

public class CandleLightPackageGenerator : MonoBehaviour, IObserver<LightPackage>, IObserver<LightFlicker, ILightPreprocess>
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;
    [SerializeField]
    LightPreprocessDelegatorManager lightPreprocessDelegatorManager;


    private ILightPreprocess lightFlickerPreprocess;

    private string LightFlickerUniqueKey { get; set; }

    private void Start()
    {
        StartCoroutine(lightPreprocessDelegatorManager.LightPreprocessDelegator.NotifyWhenActive((IObserver<MonoBehaviour, ILightPreprocess>)this, new NotificationContext()
        {
            GameObject = gameObject,
            GameObjectName = gameObject.name,
            GameObjectTag = gameObject.tag,
        }));
    }

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

    public void OnNotify(ILightPreprocess data, NotificationContext context, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        lightFlickerPreprocess = data;
    }

    public void OnKeyNotify(string key, NotificationContext context, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        LightFlickerUniqueKey = key;

        StartCoroutine(lightPreprocessDelegatorManager.LightPreprocessDelegator.NotifySubject(LightFlickerUniqueKey, (IObserver<MonoBehaviour, ILightPreprocess>)this));
    }
}