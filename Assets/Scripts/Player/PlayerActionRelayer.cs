using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CheckPoints;
using static SceneData;
using static DialoguesAndOptions;
using PlayerHittableItemsNS;
public class PlayerActionRelayer : MonoBehaviour, IObserver<Health>, IObserver<PlayerSystem>, IGameStateHandler
{
    private const int CRYSTAL_UI_INCREMENT_COUNTER = 1;

    [SerializeField] SpriteRenderer sr;
    [SerializeField] string InteractableTag;
    [SerializeField] GameObject TeleportTransition;
    [SerializeField] string[] checkpointTags;
    [SerializeField] float playerHealth;
    [SerializeField] MainThreadDispatcherEvent mainThreadDispatcherEvent;
    [SerializeField] HealthDelegator healthDelegator;

    private Animator anim;
    private float ENEMYATTACK = 5f;
    private bool pickedUp;
    private PickableItemsHandler _pickableItems;
    private SemaphoreSlim _semaphoreSlim;
    private SemaphoreSlim _semaphoreSlimForCheckpoint;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;

    public SemaphoreSlim GetCheckPointSemaphore { get => _semaphoreSlimForCheckpoint; set => _semaphoreSlimForCheckpoint = value; }
    public SemaphoreSlim GetSemaphore { get => _semaphoreSlim; set => _semaphoreSlim = value; }
    private DialogueSystem DialogueSystemFetched { get; set; }
    
    private PlayerSystem PlayerSystemReference { get; set; }

    private Health PlayerHealth { get; set; }

    private bool InSight { get; set; }

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

        StartCoroutine(healthDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = name,
            SubjectType = typeof(HealthManager).ToString()

        }, CancellationToken.None));

    }
    private void Awake()
    {
        _semaphoreSlim = new SemaphoreSlim(1); //using at two places

        _semaphoreSlimForCheckpoint = new SemaphoreSlim(1);

        _cancellationTokenSource = new CancellationTokenSource();

        _cancellationToken = _cancellationTokenSource.Token;

        _pickableItems= GameObject.FindWithTag("PickableItemsManager").GetComponent<PickableItemsHandler>();

        sr = GetComponent<SpriteRenderer>();

        anim = GetComponent<Animator>();

    }

    private async void Update()
    {
        
        if (await IsPlayerDead(PlayerHealth.CurrentHealth) && GetCheckPointSemaphore.CurrentCount!=0)
        {
            await GetCheckPointSemaphore.WaitAsync();
            anim.SetBool(PlayerAnimationConstants.DEATH, true);
            await Task.Delay(TimeSpan.FromSeconds(0.1f));
            await SceneSingleton.GetEntityListenerDelegator().ListenerDelegator<EntitiesToReset>(PlayerObserverListenerHelper.EntitiesToReset, SceneSingleton.EntitiesToReset);

            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                await SceneSingleton.GetEntityListenerDelegator().ListenerDelegator<GameObject>(PlayerObserverListenerHelper.MainPlayerListener, gameObject, lockingThread : GetCheckPointSemaphore);

            }
        }
        
    }
    private async void FixedUpdate()
    {
        if (await IfPortalExists(sr, "Portal"))
        {
            //Instantiate(TeleportTransition, transform.position, Quaternion.identity);
            StartCoroutine(WaiterFunction());
            GameStateManager.ChangeLevel(SceneManager.GetActiveScene().buildIndex);
        }

        await GetSemaphore.WaitAsync();

        await IsGameObjectInSightForDialogueTrigger(SceneSingleton.DialogueAndOptions, _cancellationToken, GetSemaphore);

        if (InSight && DialogueSystemFetched != null && !DialogueSystemFetched.DialogueSettings.DialogueConcluded)
        {
          await SceneSingleton.GetEntityListenerDelegator().ListenerDelegator<DialogueSystem>(PlayerObserverListenerHelper.DialogueSystem, DialogueSystemFetched);
        }

    }
    private Task IsGameObjectInSightForDialogueTrigger(DialoguesAndOptions dialogueAndOptions, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim)
    {
        InSight = false;
        DialogueSystemFetched = null;

        foreach (var item in dialogueAndOptions.exchange)
        {
            if (!cancellationToken.IsCancellationRequested && FindingObjects.CastRayToFindObject(gameObject, item.DialogueTriggeringEntity.EntityTag, 3f))
            {
                InSight = true;
                DialogueSystemFetched = item;
                break;
            }
        }

        semaphoreSlim.Release();

        return Task.CompletedTask;
    }
    private async void OnCollisionEnter2D(Collision2D collision) //FIX THIS TOO
    {
        if (await CanPlayerBeAttacked(SceneSingleton.PlayerHittableItems, collision.collider))
        {
            PlayerHealth.CurrentHealth -= ENEMYATTACK;
        }
    }

    private Task<bool> CanPlayerBeAttacked(PlayerHittableItemsScriptableObject scriptableObject, Collider2D collider)
    {
        foreach (var item in scriptableObject.colliderItems)
        {
            if (collider!=null && item.collider.tag == collider.tag && !item.isItBasedOnAnimationName)
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

    private Task<bool> IsPlayerDead(float health)
    {
        return health == 0 ? Task.FromResult(true) : Task.FromResult(false);
    }

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        await ItemCollisionHandler(collision);
        await CheckpointCollisionHandler(collision);

    }

    private async Task ItemCollisionHandler(Collider2D collision)
    {
        pickedUp = _pickableItems.DidPlayerCollideWithaPickableItem(collision.tag);

        if (pickedUp)
        {
            bool shouldBedisabled = _pickableItems.ShouldThisItemBeDisabled(collision.tag);

            if (shouldBedisabled)
                collision.gameObject.SetActive(false);

            bool shouldMusicBePlayed = true;

            await SceneSingleton.GetEntityListenerDelegator().ListenerDelegator<bool>(PlayerObserverListenerHelper.BoolSubjects, shouldMusicBePlayed);
        }

        await SceneSingleton.GetEntityListenerDelegator().ListenerDelegator<Collider2D>(PlayerObserverListenerHelper.ColliderSubjects, collision);

    }
    private async Task CheckpointCollisionHandler(Collider2D collision)
    {
        if (await GetOneOfTheCheckPoints(collision.tag, checkpointTags))
        {
            //call checkpoint replacement 
            Checkpoint checkpoint = await GetCheckPointFromScriptableObject(SceneSingleton.CheckPoints, collision.tag);

            collision.gameObject.SetActive(false); //turn it off

            await SceneSingleton.GetEntityListenerDelegator().ListenerDelegator<Checkpoint>(PlayerObserverListenerHelper.CheckPointsObserver, checkpoint);

        }
    }

    private Task<Checkpoint> GetCheckPointFromScriptableObject(CheckPoints checkpointsScriptableObject, string tag)
    {
        foreach(var cp in checkpointsScriptableObject.checkpoints)
        {
            if(string.Compare(tag, cp.checkpoint.transform.tag, true)==0)
            {
                return Task.FromResult(cp);
            }
        }
        return null;
    }

    private Task<bool> GetOneOfTheCheckPoints(string tag, string[] tags)
    { 
        foreach(var cpTag in tags)
        {
            if(cpTag==tag)
                return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }


    private async Task<bool> IfPortalExists(SpriteRenderer sr, string portalTag)
    {
        RaycastHit hit; //using 3D raycast because of 3D object, portal
        Vector2 pos = transform.position;

        //make it better
        int sign = await PlayerSystemReference.PlayerFlipped(transform);

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

    private void OnDisable()
    {
       _cancellationTokenSource.Cancel();
    }

    public void GameStateHandler(SceneData data)
    {
        AbstractEntity entity = GetComponent<AbstractEntity>();

        ObjectData playerData = new ObjectData(transform.tag, transform.name, transform.position, transform.rotation, entity);

        data.AddToObjectsToPersist(playerData);
    }

    public void OnNotify(Health data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerHealth = data;
    }

    public void OnNotify(PlayerSystem data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerSystemReference = data;
    }
}








