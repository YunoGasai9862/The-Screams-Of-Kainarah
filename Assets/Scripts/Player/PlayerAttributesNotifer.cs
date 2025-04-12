using UnityEngine;

[ObserverSystem(SubjectType = typeof(PlayerAttributesNotifier), ObserverType = typeof(CandleLightPackageGenerator))]
public class PlayerAttributesNotifier: MonoBehaviour, ISubject<IObserver<Transform>>
{
    [SerializeField]
    PlayerAttributesDelegator playerAttributesDelegator;

    private Transform PlayerTransform { get; set; }

    private void OnEnable()
    {
        PlayerTransform = GetComponent<Transform>();
    }

    private void Start()
    {
        playerAttributesDelegator.AddToSubjectsDict(gameObject.tag, new Subject<IObserver<Transform>>() { });

        playerAttributesDelegator.GetSubject(gameObject.tag).SetSubject(this);
    }

    public void OnNotifySubject(IObserver<Transform> data, NotificationContext notificationContext, params object[] optional)
    {
        StartCoroutine(playerAttributesDelegator.NotifyObserver(data, PlayerTransform, notificationContext));
    }
}