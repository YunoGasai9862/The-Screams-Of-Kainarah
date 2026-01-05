using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour, IObserver<IEntityHealth>
{

    [SerializeField] Image Fill;
    [SerializeField] Slider slide;
    [SerializeField] Gradient gr;

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private Health PlayerHealth { get; set; }

    private async void Start()
    {
        PlayerAttributesDelegator = await Helper.GetDelegator<PlayerAttributesDelegator>();

        PlayerAttributesDelegator.NotifySubjectWrapper(this, new ObserverContext()
        {
            Name = gameObject.name,
            Tag = gameObject.tag,
            EntityType = typeof(PlayerAttributesNotifier).ToString()
        }, CancellationToken.None);

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

    public void OnNotify(IEntityHealth data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerHealth = data.Health;
    }
}
