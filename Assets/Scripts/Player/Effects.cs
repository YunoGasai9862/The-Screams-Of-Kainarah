using Assets.Annotations;
using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using Annotations.Enums;
using Assets.Scripts.BaseScene;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(Effects), SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(Player))]
public class Effects: MonoBehaviorScene, INotify<Player>
{
    private MaterialFader MaterialFader { get; set; } = new MaterialFader();

    public SceneUtils SceneUtils { get; set; }

    private async void Start()
    {
        SceneUtils = (await (await GetBaseScene()).GetSceneUtilsAsync());

        StartCoroutine(SceneUtils.NotifySubjectWrapper(new ObserverContext<Player>()
        {
            Instance = gameObject,
            EntityType = typeof(Effects),
            SubjectType = typeof(PlayerAttributesNotifier),
        }, this));
    }

    public IEnumerator Notify(Player value)
    {
        MaterialFader.FadeFloat(new MaterialPropertyUpdate<float>()
        {
            Material = value.DefaultRendererValue.Renderer.sharedMaterial,
            PropertyName = "_FadeIn",
            Value = 1.0f
        }, 0.1f, 1);

        yield return null;
    }
}