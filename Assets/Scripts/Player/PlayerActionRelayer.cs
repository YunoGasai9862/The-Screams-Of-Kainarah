using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.Base;
using Assets.Scripts.ScenePersistence.Models;
using PlayerHittableItemsNS;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[Subject(SubjectType = typeof(PlayerActionRelayer), ContextType = typeof(Collider2D))]
[Subject(SubjectType = typeof(PlayerActionRelayer), ContextType = typeof(bool))]
[Subject(SubjectType = typeof(PlayerActionRelayer), ContextType = typeof(DialoguesAndOptions.DialogueSystem))]
[Subject(SubjectType = typeof(PlayerActionRelayer), ContextType = typeof(EntitiesToReset))]
[Observer(ObserverType = typeof(PlayerActionRelayer), SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(Player))]
[Observer(ObserverType = typeof(PlayerActionRelayer), SubjectType = typeof(EntityPoolManager), ContextType = typeof(EntityPoolManager))]
public class PlayerActionRelayer : MonoBehaviour, INotify<Player>, IGameStateHandler, INotify<EntityPoolManager>, IRequest<Collider2D>, IRequest<bool>, IRequest<DialoguesAndOptions.DialogueSystem>, IRequest<CheckPoints.Checkpoint>, IRequest<EntitiesToReset>
{
    [SerializeField] string InteractableTag;
    [SerializeField] GameObject TeleportTransition;
    [SerializeField] string[] checkpointTags;
    [SerializeField] float playerHealth;
    [SerializeField] MainThreadDispatcherEvent mainThreadDispatcherEvent;
    [SerializeField] EntitiesToReset entitiesToReset;
    [SerializeField] DialoguesAndOptions dialoguesAndOptions;
    [SerializeField] PlayerHittableItemsScriptableObject playerHittableItemsScriptableObject;

    private Animator anim;

    private bool pickedUp;

    private const float ENEMY_ATTACK = 5f;

    private const int CRYSTAL_UI_INCREMENT_COUNTER = 1;

    private const string PICKABLE_ITEMS_KEY = "PickableItems";

    private const string CHECKPOINTS_KEY = "CheckPoints";

    private Player Player { get; set; } = new Player();

    private Delegator Delegator { get; set; }

    private EntityPoolManager EntityPoolManagerInstance { get; set; }

    private PickableItems PickableItemsSO { get; set; }

    private CheckPoints CheckPointsSO { get; set; }

    private PickableItemsUtility PickableItemsUtility { get; set; }

    private void Start()
    {
        try
        {
            SceneSingleton.InsertIntoGameStateHandlerList(this);
        }
        catch (Exception ex)
        {
            Debug.Log($"Exception: {ex.StackTrace}");
        }

        StartCoroutine(Delegator.NotifySubject(new ObserverContext<Player>()
        {
            Instance = gameObject,
            SubjectType = typeof(PlayerAttributesNotifier)

        }, this));
    }
    private async void Awake()
    {
        Delegator = await Helper.GetDelegator<Delegator>();
    }

    private void Update()
    {
        if (Player.Health == null)
        {
            Debug.Log("PlayerHealth is null for [PlayerActionRelayer - Update] - exiting!");
            return;
        }

        if (IsPlayerDead(Player.Health.CurrentHealth))
        {
            anim.SetBool(PlayerAnimationField.Death.ToString(), true);

            Delegator.NotifyObserversWrapper(new SubjectContext<EntitiesToReset>()
            {
                Data = entitiesToReset,
                EntityType = typeof(PlayerActionRelayer),
            }, this);
            
             SceneSingleton.GetEntityListenerDelegator().ListenerDelegator<GameObject>(PlayerObserverListenerHelper.MainPlayerListener, gameObject, lockingThread : GetCheckPointSemaphore);
        }
        
    }
    private async void FixedUpdate()
    {
        if (await IfPortalExists("Portal"))
        {
            //Instantiate(TeleportTransition, transform.position, Quaternion.identity);
            StartCoroutine(WaiterFunction());
            GameStateManager.ChangeLevel(SceneManager.GetActiveScene().buildIndex);
        }

        DialoguesAndOptions.DialogueSystem dialogueSystem = GetDialogueSystem(dialoguesAndOptions);

        if (dialogueSystem != null && !dialogueSystem.DialogueSettings.DialogueConcluded)
        {
            Delegator.NotifyObserversWrapper(new SubjectContext<DialoguesAndOptions.DialogueSystem>()
            {
                Data = dialogueSystem,
                EntityType = typeof(PlayerActionRelayer),
            }, this);
        }

    }
    private DialoguesAndOptions.DialogueSystem GetDialogueSystem(DialoguesAndOptions dialogueAndOptions)
    {
        foreach (var item in dialogueAndOptions.exchange)
        {
            if (FindingObjects.CastRayToFindObject(gameObject, item.DialogueTriggeringEntity.EntityTag, 3f))
            {
                return item;
            }
        }

        return null;
    }
    private async void OnCollisionEnter2D(Collision2D collision) //FIX THIS TOO
    {
        if (await CanPlayerBeAttacked(playerHittableItemsScriptableObject, collision.collider))
        {
            Player.Health.CurrentHealth -= ENEMY_ATTACK;
        }
    }

    private Task<bool> CanPlayerBeAttacked(PlayerHittableItemsScriptableObject scriptableObject, Collider2D collider)
    {
        foreach (var item in scriptableObject.colliderItems)
        {
            if (collider!=null && item.collider!= null && item.collider.tag == collider.tag && !item.isItBasedOnAnimationName)
            {
                return Task.FromResult(true);
            }

            if (item.isItBasedOnAnimationName)
            {
                Animator animator = collider.transform.root.GetComponent<Animator>() ?? null; //attached to the root component always (the animator)

                if (animator != null)
                {
                    return Task.FromResult(animator.GetCurrentAnimatorStateInfo(0).IsName(item.animationName));
                }
            }
           
        }
        return Task.FromResult(false);
    }

    private bool IsPlayerDead(float health)
    {
        return health == 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemCollisionHandler(collision);

        CheckpointCollisionHandler(collision);
    }

    private void ItemCollisionHandler(Collider2D collision)
    {
        pickedUp = PickableItemsUtility.IsPickableItem(collision.tag);

        if (pickedUp)
        {
            bool shouldBedisabled = PickableItemsUtility.ShouldThisItemBeDisabled(collision.tag);

            if (shouldBedisabled)
                collision.gameObject.SetActive(false);

            Delegator.NotifyObserversWrapper(new SubjectContext<bool>()
            {
                Data = true,
                EntityType = typeof(PlayerActionRelayer),
            }, this);
        }

        await SceneSingleton.GetEntityListenerDelegator().ListenerDelegator<Collider2D>(PlayerObserverListenerHelper.ColliderSubjects, collision);

    }
    private void CheckpointCollisionHandler(Collider2D collision)
    {
        if (GetOneOfTheCheckPoints(collision.tag, checkpointTags))
        {
            //call checkpoint replacement 
            CheckPoints.Checkpoint checkpoint = GetCheckPointFromScriptableObject(CheckPointsSO, collision.tag);

            collision.gameObject.SetActive(false); //turn it off

            Delegator.NotifyObserversWrapper(new SubjectContext<CheckPoints.Checkpoint>()
            {
                Data = checkpoint,
                EntityType = typeof(PlayerActionRelayer),
            }, this);

        }
    }

    private CheckPoints.Checkpoint GetCheckPointFromScriptableObject(CheckPoints checkpointsScriptableObject, string tag)
    {
        foreach(var cp in checkpointsScriptableObject.checkpoints)
        {
            if(string.Compare(tag, cp.checkpoint.transform.tag, true)==0)
            {
                return cp;
            }
        }
        return null;
    }

    private bool GetOneOfTheCheckPoints(string tag, string[] tags)
    { 
        foreach(var cpTag in tags)
        {
            if(cpTag==tag)
                return true;
        }
        return false;
    }


    private async Task<bool> IfPortalExists(string portalTag)
    {
        RaycastHit hit; //using 3D raycast because of 3D object, portal

        Vector2 pos = transform.position;

        //make it better
        int sign = await Helper.PlayerFlipped(transform);

        pos.x = transform.position.x + sign;

        Physics.Raycast(transform.position, transform.right * sign, out hit, 1f);
        Debug.DrawRay(pos, transform.right * sign, Color.red);

        await Task.Delay(TimeSpan.FromSeconds(0));

        return hit.collider != null && hit.collider.isTrigger && hit.collider.CompareTag(portalTag);
    }

    private IEnumerator WaiterFunction()
    {
        yield return new WaitForSeconds(1f);
    }

    public void GameStateHandler(SceneData data)
    {
        AbstractEntity entity = GetComponent<AbstractEntity>();

        SceneData.ObjectData playerData = new SceneData.ObjectData(transform.tag, transform.name, transform.position, transform.rotation, entity);

        data.AddToObjectsToPersist(playerData);
    }

    public IEnumerator Notify(PickableItems value)
    {

        yield return null;
    }

    public IEnumerator Notify(Player value)
    {
        Player = value;

        yield return null;
    }

    public IEnumerator Notify(EntityPoolManager value)
    {
        EntityPoolManagerInstance = value;

        PickableItemsSO = Helper.GetFromEntityPoolManager<PickableItems>(EntityPoolManagerInstance, PICKABLE_ITEMS_KEY);

        PickableItemsUtility = new PickableItemsUtility(PickableItemsSO);

        CheckPointsSO = Helper.GetFromEntityPoolManager<CheckPoints>(EntityPoolManagerInstance, CHECKPOINTS_KEY);

        yield return null;
    }

}








