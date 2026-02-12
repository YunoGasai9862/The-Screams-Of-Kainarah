
using Assets.Annotations;
using System.Collections;
using UnityEngine;

[Observer(ObserverType = typeof(TrackPlayer), SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(IEntityTransform))]
public class TrackPlayer : MonoBehaviour, INotify<IEntityTransform>
{
    private Transform PlayerTransform { get; set; }

    private Delegator Delegator { get; set; }

    private async void Awake()
    {
        Delegator = await Helper.GetDelegator<Delegator>();
    }

    private void Start()
    {
        StartCoroutine(Delegator.NotifySubject(new ObserverContext<IEntityTransform>()
        {
            Instance = gameObject,
            SubjectType = typeof(PlayerAttributesNotifier)
        }, this));
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

    public IEnumerator Notify(IEntityTransform value)
    {
        PlayerTransform = value.Transform;

        yield return null;
    }
}
