using Assets.Annotations;
using System;
using System.Collections;
using System.Threading;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Threading.Tasks;
using UnityEngine;
using Annotations.Enums;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(MoonMovement), SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(IEntityTransform))]
public class MoonMovement : MonoBehaviour, INotify<IEntityTransform>
{
    [Header("Custom Variables")]
    [SerializeField] float moonSpeed;
    [SerializeField] float XOffset, YOffset, ZOffset;
    [SerializeField] float distanceBetweenPlayerAndMoon;

    private Delegator Delegator { get; set; }

    private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;

    private Transform PlayerTransform { get; set; }

    private async void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;

       StartCoroutine(Helper.GetDelegator<Delegator>(value => Delegator = value));

        Delegator.NotifySubjectWrapper(new ObserverContext<IEntityTransform>()
        {
            Instance = gameObject,
            EntityType = typeof(MoonMovement),
            SubjectType = typeof(PlayerAttributesNotifier)
        }, this);
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

    public IEnumerator Notify(IEntityTransform value)
    {
        PlayerTransform = value.Transform;

        yield return null;
    }
}
