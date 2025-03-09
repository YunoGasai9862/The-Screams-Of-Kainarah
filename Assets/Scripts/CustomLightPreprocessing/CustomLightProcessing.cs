using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CustomLightProcessing : MonoBehaviour, ICustomLightPreprocessing, IObserver<AsyncCoroutine>, ISubject<IObserver<LightPackage>>
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

        lightPackageDelegator.Subject.SetSubject(this);
    }

    public  IEnumerator ExecuteLightningLogic(LightPackage lightPackage, ILightPreprocess customLightPreprocessingImplementation, CancellationToken cancellationToken)
    {
        yield return new WaitUntil(() => AsyncCoroutine != null);

        //pulse etc should not be the responsiblity of Execute method - but should be checked within customlightning method 
        if (lightPackage != null)
        {
            yield return new WaitUntil(() => m_Semaphore.CurrentCount != 0);

            Debug.Log(m_Semaphore.CurrentCount);

            //npw test this tomorrow!!
            AsyncCoroutine.ExecuteAsyncCoroutine(customLightPreprocessingImplementation.GenerateCustomLighting(lightPackage, m_Semaphore, 5f)); //Async runner

            m_Semaphore.WaitAsync();
        }
    }

    public void OnNotify(AsyncCoroutine data, NotificationContext notificationContext, params object[] optional)
    {
        AsyncCoroutine = data;
    }

    public void OnNotifySubject(IObserver<LightPackage> data, NotificationContext notificationContext, params object[] optional)
    {
        //fix this
       //StartCoroutine(ExecuteLightningLogic(data,  CancellationToken));
    }
}
