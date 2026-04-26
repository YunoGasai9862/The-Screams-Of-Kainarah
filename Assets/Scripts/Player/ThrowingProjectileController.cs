using Assets.Annotations;
using CoreCode;
using System.Collections;
using UnityEngine;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using Annotations.Enums;

[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(ThrowingProjectileController), EntityType = typeof(PickableItems), ContextType = typeof(ScriptableObject))]
public class ThrowingProjectileController : MonoBehaviour, IReceiver<bool>, INotify<ScriptableObject>
{
    private const string DAGGER_ITEM_NAME = "Dagger";

    private ThrowableProjectileEvent onThrowEvent = new ThrowableProjectileEvent();

    private PlayerAttackStateMachine _playerAttackStateMachine;

    private Animator _anim;

    private PickableItemsUtility PickableItemsUtility { get; set; }

    private Delegator Delegator { get; set; }

    [SerializeField] string pickableItemClassTag;

    private void Awake()   
    {
        _anim= GetComponent<Animator>();
        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);
        ProjectileThrowAnimationEvent.AddEventListener(DidHalfAnimationPass);
    }
    private async void Start()
    {
        onThrowEvent.AddListener(CanPlayerThrowProjectile);

        Delegator = await Helper.GetDelegator<Delegator>();

        StartCoroutine(Delegator.NotifySubject(new ObserverContext<ScriptableObject>()
        {
            Instance = gameObject,
            EntityType = typeof(ThrowingProjectileController),
            SubjectType = typeof(PickableItems)

        }, this));

    }
    private async void ThrowDaggerHandler()
    {
        bool daggerExistsInInventory = await InventoryManagementSystem.Instance.DoesItemExistInInventory(DAGGER_ITEM_NAME);

        if (daggerExistsInInventory)
        {
            ThrowDagger(PickableItemsUtility.GetGameObject(DAGGER_ITEM_NAME));
        }

    }
    private void ThrowDagger(GameObject prefab)
    {
        InstantiateUtility dagger = new(prefab);

        GameObject daggerGameObject = dagger.InstantiateObject(GetDaggerPositionWithOffset(2, -1), Quaternion.identity);

        InventoryManagementSystem.Instance.RemoveInvoke(prefab.gameObject.tag); //invoking event for removal

        DaggerController controller = daggerGameObject.GetComponent<DaggerController>();

        controller.Invoke(true);

    }

    public Vector2 GetDaggerPositionWithOffset(float xOffset, float yOffset)
    {
        return IsPlayerFlipped(transform) ? new Vector2(transform.position.x - xOffset, transform.position.y + yOffset) :
            new Vector2(transform.position.x + xOffset, transform.position.y + yOffset);
    }

    public bool IsPlayerFlipped(Transform playerTransform)
    {
        return playerTransform.localScale.x < 0 ? true : false; 
    }
    public bool CancelAction()
    {
        return true;
    }

    public bool PerformAction(bool value = false)
    {
        TriggerAnimation();
        return true;
    }

    private void TriggerAnimation()
    {
        _playerAttackStateMachine.SetAttackState(PlayerAnimationField.ThrowDagger.ToString(), onThrowEvent.CanThrow);
    }

    public void CanPlayerThrowProjectile(bool canThrow)
    {
        onThrowEvent.CanThrow = canThrow;
    }
    public void InvokeThrowableProjectileEvent(bool canThrow)
    {
        onThrowEvent.Invoke(canThrow);
    }
    public void DidHalfAnimationPass()
    {
        ThrowDaggerHandler();
    }

    public IEnumerator Notify(ScriptableObject value)
    {
        PickableItemsUtility = new PickableItemsUtility((PickableItems)value);

        yield return null;
    }
}
