using Assets.Annotations;
using System.Collections;
using UnityEngine;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;

[Observer(ObserverType = typeof(CameraFollow), SubjectType = typeof(CameraShake), ContextType = typeof(bool))]
[Observer(ObserverType = typeof(CameraFollow), SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(IEntityTransform))]
public class CameraFollow : MonoBehaviour, INotify<bool>, INotify<IEntityTransform>
{
    [Header("Camera Follow Speed")]
    [SerializeField] float _cameraFollowSpeed;

    private Delegator Delegator { get; set; }

    private bool ShouldFollowPlayer { get; set; }

    private Transform PlayersTransform { get; set;}

    private async void Start()
    {
        Delegator = await Helper.GetDelegator<Delegator>();

        Delegator.NotifySubjectWrapper(new ObserverContext<bool>()
        {
            Instance = gameObject,
            SubjectType = typeof(CameraShake)
        }, this);

        Delegator.NotifySubjectWrapper(new ObserverContext<IEntityTransform>()
        {
            Instance = gameObject,
            SubjectType = typeof(PlayerAttributesNotifier)
        }, this);
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

    public IEnumerator Notify(IEntityTransform value)
    {
        PlayersTransform = value.Transform;

        yield return null;
    }

    public IEnumerator Notify(bool value)
    {
        ShouldFollowPlayer = value;

        yield return null;
    }
}
