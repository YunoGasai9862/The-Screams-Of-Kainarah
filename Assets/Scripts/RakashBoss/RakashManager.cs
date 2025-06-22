using static SceneData;
public class RakashManager : AbstractEntity, IGameStateHandler
{
    public override Health Health {

        get {

            if (Health == null)
            {
                Health = new Health()
                {
                    MaxHealth = 100f,
                    CurrentHealth = 100f,
                    EntityName = gameObject.name
                };
            }

            return Health;
        
        }; 
        
        set => Health = value;
    }

    //push health via OBSERVER pattern, then via a custom event update it in the manager class

    void Start()
    {
        SceneSingleton.InsertIntoGameStateHandlerList(this);
    }

    public override void GameStateHandler(SceneData data)
    {
        ObjectData bossData = new ObjectData(transform.tag, transform.name, transform.position, transform.rotation);

        data.AddToObjectsToPersist(bossData);
    }
}
