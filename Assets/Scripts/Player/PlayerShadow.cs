using Assets.Annotations;
using System.Collections;
using System.Threading.Tasks;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using Annotations.Enums;

[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(PlayerAttributesNotifier), EntityType = typeof(PlayerShadow), ContextType = typeof(Player))]
public class PlayerShadow : MonoBehaviour, INotify<Player>
{
    private Vector2 m_Position;
    private Vector2 m_newPosition;

    [SerializeField]
    public float initialoffsetY;
    public float initialoffsetX;

    private Delegator Delegator { get; set; }

    private Player Player { get; set; }

    private async void Awake()
    {
        m_Position = new Vector2(transform.position.x + initialoffsetX, transform.position.y + initialoffsetY);

        Delegator = await Helper.GetDelegator<Delegator>();   

        Delegator.NotifySubjectWrapper(new ObserverContext<Player>()
        {
            Instance = gameObject,
            EntityType = typeof(PlayerShadow),
            SubjectType = typeof(PlayerAttributesNotifier)
        }, this);

    }
    // Update is called once per frame
     async void Update()
    {

        if (Player == null)
        {
            Debug.Log("Player is null in PlayerShadow - skipping Update for the time being!");
            return;
        }

        m_newPosition = await ShadowObjectsNewPosition(Player.SpriteRendererValue.Renderer, Player.Transform.position, m_Position, 0.5f, 10);

        transform.position = new Vector2(m_newPosition.x, m_newPosition.y);

        m_Position = transform.position;

        Player.Transform.position = transform.parent.position;
    }

    private async Task<Vector2> ShadowObjectsNewPosition(SpriteRenderer spriteRenderer, Vector2 parentPos, Vector2 position, float offsetx, int delyForShadowInMiliseconds)
    {
        Vector2 result = new(0, 0);

        result = Helper.FlipTheObjectToFaceParent(ref spriteRenderer, parentPos, position, offsetx);

        await Task.Delay(delyForShadowInMiliseconds); //why making it zero fix the issue of getting the null exception (debug tomorrow)

        return result;

    }

    public IEnumerator Notify(Player value)
    {
        Player = value;

        yield return null;
    }
}
