using System.Collections;
using System.Threading;
using UnityEngine;

[ObserverSystem(SubjectType = typeof(CandleLightPackageGenerator), ObserverType = typeof(CustomLightProcessing))]
[ObserverSystem(SubjectType = typeof(CelestialBodiesLightPackageGenerator), ObserverType = typeof(CustomLightProcessing))]
public class CustomLightProcessing : MonoBehaviour, ICustomLightPreprocessing, IObserver<AsyncCoroutine>, IObserver<LightPackage>
{
    private AsyncCoroutine AsyncCoroutine { get; set; }

    private CancellationTokenSource CancellationTokenSource { get; set; }
    private CancellationToken CancellationToken { get; set; }

    [Header("Light Intensity Swing Values")]
    [SerializeField]
    public float maxIntensity;
    public float minIntensity;

    [Header("Async Coroutine Delegator Reference")]
    [SerializeField]
    public AsyncCoroutineDelegator asyncCoroutineDelegator;

    [Header("LightPackage Coroutine Delegator Reference")]
    [SerializeField]
    public LightPackageDelegator lightPackageDelegator;

    private SemaphoreSlim m_Semaphore = new SemaphoreSlim(1, 1);

    private void Awake()
    {
        //please put this on one object, and let other lights use that!!
        //pass the light source as well!!
        CancellationTokenSource = new CancellationTokenSource();
        CancellationToken = CancellationTokenSource.Token;
    }

    private void Start()
    {
        StartCoroutine(asyncCoroutineDelegator.NotifySubject(this));

        //notify them both :) - this way we keep the subjects asundered from each other. ALso gives us control when and which subject to ping <3 
        StartCoroutine(lightPackageDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(CandleLightPackageGenerator).ToString())));
        StartCoroutine(lightPackageDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(CelestialBodiesLightPackageGenerator).ToString())));
    }

    public IEnumerator ExecuteLightningLogic(LightPackage lightPackage, CancellationToken cancellationToken)
    {
        yield return new WaitUntil(() => AsyncCoroutine != null);

        //pulse etc should not be the responsiblity of Execute method - but should be checked within customlightning method 
        if (lightPackage != null)
        {
            yield return new WaitUntil(() => m_Semaphore.CurrentCount != 0);

            Debug.Log(m_Semaphore.CurrentCount);

            //npw test this tomorrow!!
            AsyncCoroutine.ExecuteAsyncCoroutine(lightPackage.LightPreprocess.GenerateCustomLighting(lightPackage, m_Semaphore, 5f)); //Async runner

            m_Semaphore.WaitAsync();
        }
    }

    public void OnNotify(AsyncCoroutine data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        AsyncCoroutine = data;
    }

    public void OnNotify(LightPackage data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(ExecuteLightningLogic(data, CancellationToken)); 
    }
}
