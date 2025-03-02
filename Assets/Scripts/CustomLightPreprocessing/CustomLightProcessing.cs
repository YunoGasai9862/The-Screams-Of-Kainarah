using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CustomLightProcessing : MonoBehaviour, ICustomLightPreprocessing, IObserver<AsyncCoroutine>, IObserver<LightEntity>
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

    [Header("LightProcessor Coroutine Delegator Reference")]
    [SerializeField]
    public LightProcessorDelegator lightProcessorDelegator;

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
        StartCoroutine(lightProcessorDelegator.NotifySubject(this, new NotificationContext()
        {
            GameObject = this.gameObject,
            GameObjectName = this.gameObject.name,
            GameObjectTag = this.gameObject.tag,
        }));
    }

    private IEnumerator ExecuteLightningLogic(Light2D lightSource, ILightPreprocess customLightPreprocessingImplementation, LightEntity lightEntity, CancellationToken cancellationToken)
    {
        yield return new WaitUntil(() => AsyncCoroutine != null);

        //pulse etc should not be the responsiblity of Execute method - but should be checked within customlightning method 
        if (lightEntity != null)
        {
            yield return new WaitUntil(() => m_Semaphore.CurrentCount != 0);

            Debug.Log(m_Semaphore.CurrentCount);

            //npw test this tomorrow!!
            AsyncCoroutine.ExecuteAsyncCoroutine(customLightPreprocessingImplementation.GenerateCustomLighting(lightSource, minIntensity, maxIntensity, m_Semaphore, lightEntity.InnerRadiusMin, lightEntity.InnerRadiusMax, lightEntity.OuterRadiusMin, lightEntity.OuterRadiusMax, 5f)); //Async runner

            m_Semaphore.WaitAsync();
        }
    }

    public void OnNotify(AsyncCoroutine data, NotificationContext notificationContext, params object[] optional)
    {
        AsyncCoroutine = data;
    }

    public void OnNotify(LightEntity data, NotificationContext notificationContext, params object[] optional)
    {
        //StartCoroutine(ExecuteLightningLogic(data, CancellationToken));
    }
}
