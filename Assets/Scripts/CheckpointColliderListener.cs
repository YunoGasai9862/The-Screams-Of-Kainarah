using Assets.Annotations;
using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using System.Threading;
using Annotations.Enums;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(CheckpointColliderListener), SubjectType = typeof(EntityPoolManager), ContextType = typeof(EntityPoolManager))]
[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(PlayerActionRelayer), EntityType = typeof(CheckpointColliderListener), ContextType = typeof(GameObject))]
[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(GameStateManager), EntityType = typeof(CheckpointColliderListener), ContextType = typeof(GameStateManager))]
public class CheckpointColliderListener : Scene, INotify<Player>, INotify<EntityPoolManager>, INotify<GameStateManager>
{
    private static string CHECKPOINTS_KEY = "CheckPoints";
    private EntityPoolManager EntityPoolManagerInstance { get; set; }

    private GameStateManager GameStateManagerInstance { get; set; }

    private SemaphoreSlim SemaphoreSlim { get; set; }

    private CheckPoints CheckPointsSO { get; set; }

    private MaterialFader MaterialFader { get; set; }

    private void Awake()
    {
        SemaphoreSlim = new SemaphoreSlim(1, 1);

        MaterialFader = new MaterialFader();
    }

    private IEnumerator RespawnPlayer(Player player, CheckPoints checkPointsScriptableObjectFetch)
    {
        foreach (var cp in checkPointsScriptableObjectFetch.checkpoints)
        {
            if (cp.shouldRespawn)
            {
                //TODO for the reset animation/Material
                MaterialFader.FadeFloat(new MaterialPropertyUpdate<float>()
                {
                    Material = player.DefaultRendererValue.Renderer.sharedMaterial,
                    PropertyName = "_FadeIn",
                    Value = 1.0f
                }, 0.1f, 1);

                GameStateManagerInstance.LoadLastCheckPoint(SemaphoreSlim);
            }
        }

        yield return null;
    }

    public IEnumerator Notify(Player value)
    {
        if (CheckPointsSO == null || EntityPoolManagerInstance == null)
        {
            Debug.LogError("Either the CheckPointsSO is null or the EntityPoolManagerInstance  - please get this rectified/looked into!");
            yield return null;
        }

        yield return RespawnPlayer(value, CheckPointsSO);
    }

    public IEnumerator Notify(EntityPoolManager value)
    {
        EntityPoolManagerInstance = value;

        CheckPointsSO = SceneUtils.GetFromEntityPoolManager<CheckPoints>(EntityPoolManagerInstance, CHECKPOINTS_KEY);

        yield return null;
    }

    public IEnumerator Notify(GameStateManager value)
    {
        GameStateManagerInstance = value;

        yield return null;
    }
}
