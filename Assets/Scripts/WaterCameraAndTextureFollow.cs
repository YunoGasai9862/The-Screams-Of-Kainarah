using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WaterCameraAndTextureFollow : MonoBehaviour, IObserver<IEntityTransform>
{
    [SerializeField]
    public float WaterCamerSpeed;
    public float offsetX;

    [Header("Attribute Delegator")]
    [SerializeField] PlayerAttributesDelegator playerAttributesDelegator;

    private Transform PlayerTransform { get; set; }

    private void Start()
    {
        StartCoroutine(playerAttributesDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()
        }, CancellationToken.None));
    }

    public void OnNotify(IEntityTransform data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerTransform = data.Transform;
    }

    void Update()
    {
        if (PlayerTransform == null) { 
            Debug.Log($"Player Transform is null for [WaterCameraAndTextureFollow] - exiting!");
            return;
        }

        MovementUtilities.TrackPlayer(transform, PlayerTransform, new Vector3(offsetX, transform.position.y, transform.position.z), WaterCamerSpeed);
    }
}
