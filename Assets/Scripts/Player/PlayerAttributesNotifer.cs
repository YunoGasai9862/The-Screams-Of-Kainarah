using System.Threading;
using UnityEngine;

public class PlayerAttributesNotifier: MonoBehaviour, ISubject<IObserver<Player>>
{
    private Player Player { get; set; }

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private void OnEnable()
    {
        Player = new Player() { 

            Transform = GetComponent<Transform>()
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
        StartCoroutine(PlayerAttributesDelegator.NotifyObserver(data, Player, notificationContext, cancellationToken, semaphoreSlim));
    }
}