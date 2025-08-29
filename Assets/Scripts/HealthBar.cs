using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour, IObserver<IEntityHealth>
{

    [SerializeField] Image Fill;
    [SerializeField] Slider slide;
    [SerializeField] Gradient gr;
    [SerializeField] string TargetEntityTag;

    [Header("Attribute Delegator")]
    [SerializeField] PlayerAttributesDelegator playerAttributesDelegator;

    private Health PlayerHealth { get; set; }

    private void Start()
    {
        StartCoroutine(playerAttributesDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()
        }, CancellationToken.None));

        Fill.color = gr.Evaluate(slide.value);
    }
    void Update()
    {
        if (PlayerHealth == null)
        {
            Debug.Log($"PlayerHealth is null - HealthBar - Skipping!");
            return;
        }

         TrackHealth(PlayerHealth);      
    }

    private void TrackHealth(Health health)
    {
        slide.value = health.CurrentHealth;

        Fill.color = gr.Evaluate(slide.value / 100.0f);
    }

    public void OnNotify(IEntityHealth data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerHealth = data.Health;
    }
}
