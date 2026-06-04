
using Assets.Annotations;
using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using Annotations.Enums;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(WaterCameraAndTextureFollow), SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(IEntityTransform))]
public class WaterCameraAndTextureFollow : Scene, INotify<IEntityTransform>
{
    [SerializeField]
    public float WaterCamerSpeed;
    public float offsetX;

    private Delegator Delegator { get; set; }

    private Transform PlayerTransform { get; set; }

    private async void Start()
    {
       StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));

        Delegator.NotifySubjectWrapper(new ObserverContext<IEntityTransform>()
        {
            Instance = gameObject,
            EntityType = typeof(WaterCameraAndTextureFollow),
            SubjectType = typeof(PlayerAttributesNotifier)
        }, this);
    }

    void Update()
    {
        if (PlayerTransform == null) { 
            Debug.Log($"Player Transform is null for [WaterCameraAndTextureFollow] - exiting!");
            return;
        }

        MovementUtilities.TrackPlayer(transform, PlayerTransform, new Vector3(offsetX, transform.position.y, transform.position.z), WaterCamerSpeed);
    }

    public IEnumerator Notify(IEntityTransform value)
    {
        PlayerTransform = value.Transform;

        yield return null;
    }
}
