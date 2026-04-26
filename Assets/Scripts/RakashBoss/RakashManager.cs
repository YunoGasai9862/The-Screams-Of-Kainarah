using Assets.Annotations;
using Assets.Scripts.ScenePersistence.Models;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Collections;
using Annotations.Enums;

[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(RakashManager), ContextType = typeof(Health))]
public class RakashManager : AbstractEntity, IGameStateHandler, IRequest<Health>, Assets.Scripts.Interfaces.Mediator.Base.INotify<IGameStateHandler>
{
    private Delegator Delegator { get; set; }

    public override Health Health { get; set; }

    private void Awake()
    {
        Health = new Health()
        {
            MaxHealth = 100f,
            CurrentHealth = 100f,
            EntityName = gameObject.name
        };
    }

    private async void Start()
    {
        Delegator = await Helper.GetDelegator<Delegator>();
    }

    public override void GameStateHandler(SceneData data)
    {
        data.AddToObjectsToPersist(new SceneData.ObjectData(transform.tag, transform.name, transform.position, transform.rotation));
    }

    public IEnumerator Request()
    {
        StartCoroutine(Delegator.NotifyObservers(new SubjectContext<Health>()
        {
            EntityType = typeof(RakashManager),
            Data = Health

        }, this));

        yield return null;
    }

}
