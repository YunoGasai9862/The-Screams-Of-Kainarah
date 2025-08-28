using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour, IObserver<Player>
{

    [SerializeField] Image Fill;
    [SerializeField] Slider slide;
    [SerializeField] Gradient gr;
    [SerializeField] string TargetEntityTag;

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

        Fill.color = gr.Evaluate(slide.value);
    }
    void Update()
    {
        if(_targetGameObject==null)
        {
            _targetGameObject = GameObject.FindGameObjectWithTag(TargetEntityTag);
            _targetEntity = _targetGameObject.GetComponent<AbstractEntity>();
        }
        
        if(_targetEntity!=null)
           TrackHealth(_targetEntity);      
    }

    private void TrackHealth(AbstractEntity abstractEntity)
    {
        slide.value = abstractEntity.Health.CurrentHealth;

        Fill.color = gr.Evaluate(slide.value / 100.0f);
    }

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        throw new System.NotImplementedException();
    }
}
