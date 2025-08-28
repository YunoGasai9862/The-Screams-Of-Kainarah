using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WaterCameraAndTextureFollow : MonoBehaviour, IObserver<Player>
{
    [SerializeField]
    public float WaterCamerSpeed;
    public float offsetX;

    [Header("Attribute Delegator")]
    [SerializeField] PlayerAttributesDelegator playerAttributesDelegator;

    private Player Player { get; set; }

    private void Start()
    {
        StartCoroutine(playerAttributesDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()
        }, CancellationToken.None));
    }

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player.Transform = data.Transform;
    }

    void Update()
    {
        if (Player == null)
        {
            Debug.Log($"Player Transform is null for [WaterCameraAndTextureFollow] - exiting!");
            return;
        }

        MovementUtilities.TrackPlayer(transform, Player.Transform, new Vector3(offsetX, transform.position.y, transform.position.z), WaterCamerSpeed);
    }
}
