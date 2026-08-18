using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using Assets.Scripts.BaseScene;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(HealthBar), SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(IEntityHealth))]
public class HealthBar : MonoBehaviorScene, INotify<IEntityHealth>
{

    [SerializeField] Image Fill;
    [SerializeField] Slider slide;
    [SerializeField] Gradient gr;

    private SceneUtils SceneUtils { get; set; }

    private Health PlayerHealth { get; set; }

    private async void Start()
    {
        SceneUtils = (await (await GetBaseScene()).GetSceneUtilsAsync());

        StartCoroutine(SceneUtils.NotifySubjectWrapper(new ObserverContext<IEntityHealth>()
        {
            Instance = gameObject,
            EntityType = typeof(HealthBar),
            SubjectType = typeof(PlayerAttributesNotifier)
        }, this));

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
