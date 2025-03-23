using System.Threading;
using UnityEngine;

[Subject(typeof(LightFlicker))]

public class CandleLightPackageGenerator : MonoBehaviour, IObserver<LightPackage>, IObserver<MonoBehaviour, ILightPreprocess>
{
    [SerializeField]
    LightPackageDelegator lightPackageDelegator;
    [SerializeField]
    LightPreprocessDelegatorManager lightPreprocessDelegatorManager;


    private ILightPreprocess lightFlickerPreprocess;

    private string LightFlickerUniqueKey { get; set; }

    private void Start()
    {
        //introduce a 3rd type now bilal for casting!!
        StartCoroutine(lightPreprocessDelegatorManager.LightPreprocessDelegator.NotifyWhenActive(this, new NotificationContext()
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

        StartCoroutine(lightPreprocessDelegatorManager.LightPreprocessDelegator.NotifySubject(LightFlickerUniqueKey, this));
    }

}