using CoreCode;
using PlayerAnimationHandler;
using UnityEngine;

public class ThrowingProjectileController : MonoBehaviour, IReceiver<bool>
{
    private const string DAGGER_ITEM_NAME = "Dagger";

    private ThrowableProjectileEvent onThrowEvent = new ThrowableProjectileEvent();

    private PickableItemsHandler _pickableItems;
    private PlayerAttackStateMachine _playerAttackStateMachine;
    private Animator _anim;

    [SerializeField] string pickableItemClassTag;
    private void Awake()
    {
        _anim= GetComponent<Animator>();
        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);
    }
    private void Start()
    {
        _pickableItems = GameObject.FindWithTag(pickableItemClassTag).GetComponent<PickableItemsHandler>();

        onThrowEvent.AddListener(CanPlayerThrowProjectile);
    }
    private async void ThrowDaggerHandler()
    {
        bool daggerExistsInInventory = await InventoryManagementSystem.Instance.DoesItemExistInInventory(DAGGER_ITEM_NAME);

        _playerAttackStateMachine.SetAttackState(AnimationConstants.THROW_DAGGER, onThrowEvent.CanThrow);

        if (daggerExistsInInventory)
        {
            ThrowDagger(_pickableItems.ReturnGameObjectForTheKey(DAGGER_ITEM_NAME));
        }

    }
    private void ThrowDagger(GameObject prefab)
    {
        InstantiatorController dagger = new(prefab);

        GameObject daggerGameObject = dagger.InstantiateGameObject(GetDaggerPositionWithOffset(2, -1), Quaternion.identity);

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
        ThrowDaggerHandler();
        return true;
    }
    public void CanPlayerThrowProjectile(bool canThrow)
    {
        onThrowEvent.CanThrow = canThrow;
    }
    public void InvokeThrowableProjectileEvent(bool canThrow)
    {
        onThrowEvent.Invoke(canThrow);
    }

}
