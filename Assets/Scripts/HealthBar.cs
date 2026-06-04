using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(HealthBar), SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(IEntityHealth))]
public class HealthBar : Scene, INotify<IEntityHealth>
{

    [SerializeField] Image Fill;
    [SerializeField] Slider slide;
    [SerializeField] Gradient gr;

    private Delegator Delegator { get; set; }

    private Health PlayerHealth { get; set; }

    private async void Start()
    {
       StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));

        Delegator.NotifySubjectWrapper(new ObserverContext<IEntityHealth>()
        {
            Instance = gameObject,
            EntityType = typeof(HealthBar),
            SubjectType = typeof(PlayerAttributesNotifier)
        }, this);

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

    public IEnumerator Notify(IEntityHealth value)
    {
        PlayerHealth = value.Health;

        yield return null;
    }
}
