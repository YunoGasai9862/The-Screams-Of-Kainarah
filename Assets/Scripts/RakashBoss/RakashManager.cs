using Assets.Annotations;
using Assets.Scripts.Interfaces;
using Assets.Scripts.ScenePersistence.Models;
using System.Collections;

[Subject(SubjectType = typeof(RakashManager), ContextType = typeof(Health))]
public class RakashManager : AbstractEntity, IGameStateHandler, IRequest<Health>
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

        SceneSingleton.InsertIntoGameStateHandlerList(this);
    }

    public override void GameStateHandler(SceneData data)
    {
        data.AddToObjectsToPersist(new SceneData.ObjectData(transform.tag, transform.name, transform.position, transform.rotation));
    }

    public IEnumerator Request()
    {
        //we need to consider a single instance later on
        StartCoroutine(Delegator.NotifyObservers(new SubjectContext<Health>()
        {
            EntityType = typeof(RakashManager),
            Data = Health

        }, this));

        yield return null;
    }
}
