
using Assets.Annotations;
using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using Annotations.Enums;
using Assets.Scripts.BaseScene;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(TrackPlayer), SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(IEntityTransform))]
public class TrackPlayer : MonoBehaviorScene, INotify<IEntityTransform>
{
    private Transform PlayerTransform { get; set; }

    private SceneUtils SceneUtils { get; set; }

    private async void Awake()
    {
        SceneUtils = (await (await GetBaseScene()).GetSceneUtilsAsync());
    }

    private void Start()
    {
        StartCoroutine(SceneUtils.NotifySubjectWrapper(new ObserverContext<IEntityTransform>()
        {
            Instance = gameObject,
            EntityType = typeof(TrackPlayer),
            SubjectType = typeof(PlayerAttributesNotifier)
        }, this));
    }

    void Update()
    {
        if (PlayerTransform == null)
        {
            Debug.Log($"Player Transform is null for [TrackPlayer] - exiting!");
            return;
        }

        MovementUtilities.TrackPlayer(transform, PlayerTransform, new Vector3(0, 25, 0), 0f);
    }

    public IEnumerator Notify(IEntityTransform value)
    {
        PlayerTransform = value.Transform;

        yield return null;
    }
}
