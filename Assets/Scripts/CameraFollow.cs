using System.Threading;
using UnityEngine;

public class CameraFollow : MonoBehaviour, IObserver<bool>, IObserver<Player>
{
    [Header("Camera Follow Speed")]
    [SerializeField] float _cameraFollowSpeed;

    [Header("Generic Float Delegator")]
    [SerializeField] FlagDelegator flagDelegator;

    [Header("Attribute Delegator")]
    [SerializeField] PlayerAttributesDelegator playerAttributesDelegator;

    private bool ShouldFollowPlayer { get; set; }

    private Transform PlayersTransform { get; set;}

    private void Start()
    {
        StartCoroutine(flagDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(CameraShake).ToString()
        }, CancellationToken.None));

        StartCoroutine(playerAttributesDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()
        }, CancellationToken.None));
    }

    void Update()
    {
        if(ShouldFollowPlayer)
        {
            FollowPlayer.TrackPlayer(transform, PlayersTransform.transform, new Vector3(0, 5, 0), _cameraFollowSpeed);
        }
    }

    public void OnNotify(bool data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        ShouldFollowPlayer = data;
    }

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayersTransform = data.Transform;
    }
}
