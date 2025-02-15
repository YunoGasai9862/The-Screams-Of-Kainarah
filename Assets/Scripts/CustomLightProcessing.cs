using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CustomLightProcessing : MonoBehaviour, IObserverAsync<LightEntity>, IObserver<AsyncCoroutine>
{
    private Light2D m_light;
    private AsyncCoroutine AsyncCoroutine { get; set; }

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
    public LightObserverPattern _subject;

    [Header("Async Coroutine Delegator Reference")]
    public AsyncCoroutineDelegator asyncCoroutineDelegator;

    private SemaphoreSlim m_Semaphore;

    private void Awake()
    {
        m_light = GetComponent<Light2D>();
    }

    private void Start()
    {
        StartCoroutine(asyncCoroutineDelegator.NotifySubject(this));
    }

    private void OnEnable()
    {
        if (anySubjectThatIsNotifyingTheLight)
            _subject.AddObserver(this);
    }

    private void OnDisable()
    {
        if (anySubjectThatIsNotifyingTheLight)
            _subject.RemoveObserver(this);
    }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public virtual async Task OnNotify(LightEntity Data, CancellationToken _cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        StartCoroutine(ExecuteLightningLogic(Data, _cancellationToken));
    }

    private IEnumerator ExecuteLightningLogic(LightEntity lightEntity, CancellationToken cancellationToken)
    {
        yield return new WaitUntil(() => AsyncCoroutine != null);

        if (lightEntity != null)
        {
            if (lightEntity.LightName == transform.parent.name && lightEntity.UseCustomTinkering)
            {
                m_Semaphore = new SemaphoreSlim(0);

                AsyncCoroutine.ExecuteAsyncCoroutine(customLightPreprocessingImplementation.LightCustomPreprocess().GenerateCustomLighting(m_light, minIntensity, maxIntensity, m_Semaphore, lightEntity.InnerRadiusMin, lightEntity.InnerRadiusMax, lightEntity.OuterRadiusMin, lightEntity.OuterRadiusMax)); //Async runner

                m_Semaphore.WaitAsync();
            }

            if (lightEntity.LightName == transform.parent.name && !lightEntity.UseCustomTinkering)
            {
                StopAllCoroutines(); //the fix!
            }
        }
    }

    public void OnNotify(AsyncCoroutine data, params object[] optional)
    {
        AsyncCoroutine = data;
    }
}
