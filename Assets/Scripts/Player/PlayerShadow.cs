using Assets.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

[Observer(SubjectType = typeof(PlayerAttributesNotifier), ObserverType = typeof(PlayerShadow), DataType = typeof(Player))]
public class PlayerShadow : MonoBehaviour, INotify<Player>
{
    private Vector2 m_Position;
    private Vector2 m_newPosition;
    private CancellationToken _token;
    private CancellationTokenSource _tokenSource;

    [SerializeField]
    public float initialoffsetY;
    public float initialoffsetX;

    private Delegator Delegator { get; set; }

    private Player Player { get; set; }

    private async void Awake()
    {
        m_Position = new Vector2(transform.position.x + initialoffsetX, transform.position.y + initialoffsetY);
        _tokenSource= new CancellationTokenSource();
        _token = _tokenSource.Token;

        Delegator = await Helper.GetDelegator<Delegator>();   

        Delegator.NotifySubjectWrapper(new ObserverContext<Player>()
        {
            Instance = gameObject,
            EntityType = typeof(PlayerShadow),
            SubjectType = typeof(PlayerAttributesNotifier)
        }, this, CancellationToken.None);

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

        if(!_token.IsCancellationRequested) //extra check due to async programming
        {
            transform.position = new Vector2(m_newPosition.x, m_newPosition.y); //updates it

            m_Position = transform.position;

            Player.Transform.position = transform.parent.position;
        }
    }

    private async Task<Vector2> ShadowObjectsNewPosition(SpriteRenderer spriteRenderer, Vector2 parentPos, Vector2 position, float offsetx, int delyForShadowInMiliseconds)
    {
        Vector2 result = new(0, 0);

        result = Helper.FlipTheObjectToFaceParent(ref spriteRenderer, parentPos, position, offsetx);

        await Task.Delay(delyForShadowInMiliseconds, _token); //why making it zero fix the issue of getting the null exception (debug tomorrow)

        return result;

    }

    private void OnDisable()
    {
        _tokenSource.Cancel();

    }

    public Task Notify(Player value)
    {
        Player = value;

        return Task.CompletedTask;
    }
}
