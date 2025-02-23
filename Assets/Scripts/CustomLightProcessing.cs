using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CustomLightProcessing : MonoBehaviour, IObserver<AsyncCoroutine>, IObserver<LightEntity>
{
    private Light2D m_light;
    private AsyncCoroutine AsyncCoroutine { get; set; }

    private CancellationTokenSource CancellationTokenSource { get; set; }
    private CancellationToken CancellationToken { get; set; }

    [Header("Light Intensity Swing Values")]
    [SerializeField]
    public float maxIntensity;
    public float minIntensity;

    [Header("Add CustomLightProcessing Implementation")]
    [SerializeField]
    public LightPreProcessWrapper customLightPreprocessingImplementation;

    [Header("Add the Subject which willl be responsible for notifying")]
    public bool anySubjectThatIsNotifyingTheLight;
    [HideInInspector]
    //public LightObserverPattern _subject;

    [Header("Async Coroutine Delegator Reference")]
    public AsyncCoroutineDelegator asyncCoroutineDelegator;

    private SemaphoreSlim m_Semaphore = new SemaphoreSlim(1, 1);

    private void Awake()
    {
        m_light = GetComponent<Light2D>();
        CancellationTokenSource = new CancellationTokenSource();
        CancellationToken = CancellationTokenSource.Token;
    }

    private void Start()
    {
        StartCoroutine(asyncCoroutineDelegator.NotifySubject(this));
    }

    private IEnumerator ExecuteLightningLogic(LightEntity lightEntity, CancellationToken cancellationToken)
    {
        yield return new WaitUntil(() => AsyncCoroutine != null);

        if (lightEntity != null)
        {
            if (lightEntity.LightName == transform.parent.name && lightEntity.UseCustomTinkering)
            {
                yield return new WaitUntil(() => m_Semaphore.CurrentCount != 0);

                Debug.Log(m_Semaphore.CurrentCount);

                AsyncCoroutine.ExecuteAsyncCoroutine(customLightPreprocessingImplementation.LightCustomPreprocess().GenerateCustomLighting(m_light, minIntensity, maxIntensity, m_Semaphore, lightEntity.InnerRadiusMin, lightEntity.InnerRadiusMax, lightEntity.OuterRadiusMin, lightEntity.OuterRadiusMax, 5f)); //Async runner

                m_Semaphore.WaitAsync();
            }

            if (lightEntity.LightName == transform.parent.name && !lightEntity.UseCustomTinkering)
            {
                StopAllCoroutines(); //the fix!
            }
        }
    }

    public void OnNotify(AsyncCoroutine data, NotificationContext notificationContext, params object[] optional)
    {
        AsyncCoroutine = data;
    }

    public void OnNotify(LightEntity data, NotificationContext notificationContext, params object[] optional)
    {
        StartCoroutine(ExecuteLightningLogic(data, CancellationToken));
    }
}
