using CoreCode;
using PlayerAnimationHandler;
using UnityEngine;

public class ThrowingProjectileController : MonoBehaviour, IReceiver<bool>
{
    private const string DAGGER_ITEM_NAME = "Dagger";

    public ThrowableProjectileEvent onThrowEvent = new ThrowableProjectileEvent();

    private SpriteRenderer _spriteRenderer;
    private PickableItemsHandler _pickableItems;
    private PlayerAttackStateMachine _playerAttackStateMachine;
    private Animator _anim;

    [SerializeField] string pickableItemClassTag;
    private void Awake()
    {
        _anim= GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);
    }
    private void Start()
    {
        _pickableItems = GameObject.FindWithTag(pickableItemClassTag).GetComponent<PickableItemsHandler>();

        onThrowEvent.AddListener(CanPlayerThrowProjectile);
    }
    private async void ThrowDaggerHandler()
    {
        _playerAttackStateMachine.SetAttackState(AnimationConstants.THROW_DAGGER, onThrowEvent.CanThrow);

        bool doesDaggerExistInInventory = await InventoryManagementSystem.Instance.DoesItemExistInInventory(DAGGER_ITEM_NAME);

        if (doesDaggerExistInInventory)
            ThrowDagger(_pickableItems.ReturnGameObjectForTheKey(DAGGER_ITEM_NAME));

    }
    private void ThrowDagger(GameObject prefab)
    {
        GameObjectInstantiator _daggerInstantiator = new(prefab);

        //fix dagger throw timing
        GameObject _daggerGameObject = _daggerInstantiator.InstantiateGameObject(GetDaggerPositionWithOffset(2, -1), Quaternion.identity);

        InventoryManagementSystem.Instance.RemoveInventoryItemEvent.Invoke(prefab.gameObject.tag); //invoking event for removal

        //fix this. Why reference it here??
        //_daggerGameObject.GetComponent<AttackEnemy>().throwDagger = true; //fix this
    }

    public Vector2 GetDaggerPositionWithOffset(float xOffset, float yOffset)
    {
        return IsPlayerFlipped(_spriteRenderer) ? new Vector2(transform.position.x - xOffset, transform.position.y + yOffset) :
            new Vector2(transform.position.x + xOffset, transform.position.y + yOffset);
    }

    public bool IsPlayerFlipped(SpriteRenderer _sr)
    {
        return _sr.flipX;
    }
    public bool CancelAction()
    {
        return true;
    }

    public bool PerformAction(bool value = false)
    {
        ThrowDaggerHandler();
        return true;
    }
    public void CanPlayerThrowProjectile(bool canThrow)
    {
        onThrowEvent.CanThrow = canThrow;
    }
}
