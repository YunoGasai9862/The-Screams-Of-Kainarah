using System.Threading;
using UnityEngine;

public class PlayerAttributesNotifier: MonoBehaviour, ISubject<Player>
{
    private Player Player { get; set; }

    private Health PlayerHealth { get; set; }

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private async void OnEnable()
    {
        PlayerHealth = new Health()
        {
            MaxHealth = 100f,
            CurrentHealth = 100f,
            EntityName = name
        };

        Player = new Player() {

            Transform = GetComponent<Transform>(),
            Animator = GetComponent<Animator>(),
            Collider = GetComponent<CapsuleCollider2D>(),
            SpriteRendererValue = new Player.SpriteRenderer()
            {
                Renderer = GetComponent<SpriteRenderer>()
            },
            DefaultRendererValue = new Player.DefaultRenderer()
            { 
                Renderer = GetComponent<Renderer>()
            }, 
            Rigidbody = GetComponent<Rigidbody2D>(),
            Health = PlayerHealth,
        };  

        PlayerAttributesDelegator = await Helper.GetDelegator<PlayerAttributesDelegator>();
    }

    private void Start()
    {
        PlayerAttributesDelegator.AddToSubjectsDict(typeof(PlayerAttributesNotifier).ToString(), gameObject.name, new Subject<Player>(this, typeof(PlayerAttributesNotifier)));
    }

    public void OnNotifySubject(IObserver<Player> data, Context context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(PlayerAttributesDelegator.NotifyObserver(data, Player, context, cancellationToken, semaphoreSlim));
    }
}