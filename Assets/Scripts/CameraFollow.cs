using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CameraFollow : MonoBehaviour, IObserver<bool>, IObserver<IEntityTransform>
{
    [Header("Camera Follow Speed")]
    [SerializeField] float _cameraFollowSpeed;

    private FlagDelegator FlagDelegator { get; set; }

    private PlayerAttributesDelegator PlayerAttributesDelegator {get; set; }

    private bool ShouldFollowPlayer { get; set; }

    private Transform PlayersTransform { get; set;}

    private async void Start()
    {
        PlayerAttributesDelegator = await Helper.GetDelegator<PlayerAttributesDelegator>();

        FlagDelegator = await Helper.GetDelegator<FlagDelegator>();

        FlagDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(CameraShake).ToString()
        }, CancellationToken.None);

        PlayerAttributesDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()
        }, CancellationToken.None);
    }

    void Update()
    {
        if (PlayersTransform == null)
        {
            Debug.Log($"Player Transform is null for [CameraFollow] - exiting!");

            return;
        }

        if(ShouldFollowPlayer)
        {
            MovementUtilities.TrackPlayer(transform, PlayersTransform.transform, new Vector3(0, 5, 0), _cameraFollowSpeed);
        }
    }

    public void OnNotify(bool data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        ShouldFollowPlayer = data;
    }

    public void OnNotify(IEntityTransform data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayersTransform = data.Transform;
    }
}
