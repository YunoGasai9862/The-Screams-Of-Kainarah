using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MoonMovement : MonoBehaviour, IObserver<IEntityTransform>
{
    [Header("Custom Variables")]
    [SerializeField] float moonSpeed;
    [SerializeField] float XOffset, YOffset, ZOffset;
    [SerializeField] float distanceBetweenPlayerAndMoon;

    private PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;

    private Transform PlayerTransform { get; set; }

    private async void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;

        PlayerAttributesDelegator = await Helper.GetDelegator<PlayerAttributesDelegator>();

        PlayerAttributesDelegator.NotifySubjectWrapper(this, new ObserverContext()
        {
            Name = gameObject.name,
            Tag = gameObject.tag,
            SubjectType = typeof(PlayerAttributesNotifier)
        }, CancellationToken.None);
    }
    async void Update()
    {
        if (PlayerTransform == null)
        {
            Debug.Log($"Player Transform is null for [TrackPlayer] - exiting!");
            return;
        }

        await FollowTarget(gameObject.transform, PlayerTransform, new Vector3(XOffset + distanceBetweenPlayerAndMoon, YOffset, ZOffset), moonSpeed);
    }

    private async Task<bool> FollowTarget(Transform self, Transform targetToFollow, Vector3 offset, float speed)
    {

        await semaphoreSlim.WaitAsync();
        await Task.Delay(TimeSpan.FromSeconds(0f));
        if (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                MovementUtilities.TrackPlayer(self, targetToFollow, offset, speed);
            }
            catch (OperationCanceledException ex)
            {
                Debug.LogException(ex);
                return false;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        return true;
    }

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
        semaphoreSlim.Release();
    }

    public void OnNotify(IEntityTransform data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerTransform = data.Transform;
    }
}
