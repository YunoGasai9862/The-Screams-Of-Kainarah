using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerShadow : MonoBehaviour, IObserver<Player>
{
    private Vector2 m_Position;
    private Vector2 m_newPosition;
    private CancellationToken _token;
    private CancellationTokenSource _tokenSource;

    [SerializeField]
    public float initialoffsetY;
    public float initialoffsetX;

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private Player Player { get; set; } = new Player();

    private void Awake()
    {
        m_Position = new Vector2(transform.position.x + initialoffsetX, transform.position.y + initialoffsetY);
        _tokenSource= new CancellationTokenSource();
        _token = _tokenSource.Token;

        PlayerAttributesDelegator = Helper.GetDelegator<PlayerAttributesDelegator>();   

        StartCoroutine(PlayerAttributesDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()
        }, CancellationToken.None));

    }
    // Update is called once per frame
     async void Update()
    {
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

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player = data;
    }
}
