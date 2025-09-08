using System.Threading;
using UnityEngine;

public class PlayerAttributesNotifier: MonoBehaviour, ISubject<IObserver<Player>>
{
    private Player Player { get; set; }

    private Health PlayerHealth { get; set; }

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private void OnEnable()
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

        PlayerAttributesDelegator = Helper.GetDelegator<PlayerAttributesDelegator>();
    }

    private void Start()
    {
        PlayerAttributesDelegator.AddToSubjectsDict(typeof(PlayerAttributesNotifier).ToString(), gameObject.name, new Subject<IObserver<Player>>());

        PlayerAttributesDelegator.GetSubsetSubjectsDictionary(typeof(PlayerAttributesNotifier).ToString())[gameObject.name].SetSubject(this);
    }

    public void OnNotifySubject(IObserver<Player> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        Debug.Log($"Player In PlaterAttributesNotifier {Player}");

        StartCoroutine(PlayerAttributesDelegator.NotifyObserver(data, Player, notificationContext, cancellationToken, semaphoreSlim));
    }
}