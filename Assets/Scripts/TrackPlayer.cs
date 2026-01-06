
using System.Threading;
using UnityEngine;

public class TrackPlayer : MonoBehaviour, IObserver<IEntityTransform>
{
    [Header("Attribute Delegator")]
    [SerializeField] PlayerAttributesDelegator playerAttributesDelegator;

    private Transform PlayerTransform { get; set; }

    private void Start()
    {
        StartCoroutine(playerAttributesDelegator.NotifySubject(this, new ObserverContext()
        {
            Name = gameObject.name,
            Tag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier)
        }, CancellationToken.None));
    }

    void Update()
    {
        if (PlayerTransform == null)
        {
            Debug.Log($"Player Transform is null for [TrackPlayer] - exiting!");
            return;
        }

        MovementUtilities.TrackPlayer(transform, PlayerTransform, new Vector3(0, 25, 0), 0f);
    }

    public void OnNotify(IEntityTransform data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerTransform = data.Transform;
    }
}
