using System.Threading;
using UnityEngine;

public class CameraFollow : MonoBehaviour, IObserver<bool>
{
    [Header("Camera Follow Speed")]
    [SerializeField] float _cameraFollowSpeed;

    [Header("Generic Float Delegator")]
    [SerializeField] FlagDelegator flagDelegator;

    private bool ShouldFollowPlayer { get; set; }

    void Update()
    {
        if(ShouldFollowPlayer)
        {
            FollowPlayer.TrackPlayer(transform, 0, 5, 0, _cameraFollowSpeed);
        }
    }

    public void OnNotify(bool data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        ShouldFollowPlayer = data;
    }
}
