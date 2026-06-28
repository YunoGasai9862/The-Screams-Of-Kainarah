using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Collections;
using Annotations.Enums;
using Assets.Scripts.GameState.Models;

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
       StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));
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
