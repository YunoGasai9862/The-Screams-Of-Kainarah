using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using Assets.Scripts.ScenePersistence.Models;
using PlayerHittableItemsNS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[Subject(SubjectType = typeof(PlayerActionRelayer), ContextType = typeof(Collider2D))]
[Subject(SubjectType = typeof(PlayerActionRelayer), ContextType = typeof(bool))]
[Subject(SubjectType = typeof(PlayerActionRelayer), ContextType = typeof(DialoguesAndOptions.DialogueSystem))]
[Subject(SubjectType = typeof(PlayerActionRelayer), ContextType = typeof(EntitiesToReset))]
[Observer(ObserverType = typeof(PlayerActionRelayer), SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(Player))]
[Observer(ObserverType = typeof(PlayerActionRelayer), SubjectType = typeof(PickableItems), ContextType = typeof(ScriptableObject))]
public class PlayerActionRelayer : MonoBehaviour, INotify<Player>, IGameStateHandler, INotify<ScriptableObject>, IRequest<Collider2D>, IRequest<bool>, IRequest<DialoguesAndOptions.DialogueSystem>, IRequest<EntitiesToReset>
{
    private const int CRYSTAL_UI_INCREMENT_COUNTER = 1;

    [SerializeField] string InteractableTag;
    [SerializeField] GameObject TeleportTransition;
    [SerializeField] string[] checkpointTags;
    [SerializeField] float playerHealth;
    [SerializeField] MainThreadDispatcherEvent mainThreadDispatcherEvent;

    private Animator anim;
    private float ENEMYATTACK = 5f;
    private bool pickedUp;
    private PickableItemsUtility _pickableItemsUtility;
    private DialoguesAndOptions.DialogueSystem DialogueSystemFetched { get; set; }

    private Player Player { get; set; } = new Player();

    private bool InSight { get; set; }

    private Delegator Delegator { get; set; }

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

        StartCoroutine(Delegator.NotifySubject(this, new ObserverContext()
        {
            Instance = gameObject,
            SubjectType = typeof(PlayerAttributesNotifier)

        }, CancellationToken.None));

        StartCoroutine(Delegator.NotifySubject(this, new ObserverContext()
        {
            Instance = gameObject,
            SubjectType = typeof(PickableItems)

        }, CancellationToken.None));
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

            SceneSingleton.GetEntityListenerDelegator().ListenerDelegator<EntitiesToReset>(PlayerObserverListenerHelper.EntitiesToReset, SceneSingleton.EntitiesToReset);

            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                SceneSingleton.GetEntityListenerDelegator().ListenerDelegator<GameObject>(PlayerObserverListenerHelper.MainPlayerListener, gameObject, lockingThread : GetCheckPointSemaphore);

            }
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
            Player.Health.CurrentHealth -= ENEMYATTACK;
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

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        await ItemCollisionHandler(collision);
        await CheckpointCollisionHandler(collision);

    }

    private async Task ItemCollisionHandler(Collider2D collision)
    {
        pickedUp = _pickableItemsUtility.IsPickableItem(collision.tag);

        if (pickedUp)
        {
            bool shouldBedisabled = _pickableItemsUtility.ShouldThisItemBeDisabled(collision.tag);

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
            CheckPoints.Checkpoint checkpoint = await GetCheckPointFromScriptableObject(SceneSingleton.CheckPoints, collision.tag);

            collision.gameObject.SetActive(false); //turn it off

            await SceneSingleton.GetEntityListenerDelegator().ListenerDelegator<>(PlayerObserverListenerHelper.CheckPointsObserver, checkpoint);

        }
    }

    private Task<CheckPoints.Checkpoint> GetCheckPointFromScriptableObject(CheckPoints checkpointsScriptableObject, string tag)
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

    public IEnumerator Notify(ScriptableObject value)
    {
        _pickableItemsUtility = new PickableItemsUtility((PickableItems)value);

        yield return null;
    }

    public IEnumerator Notify(Player value)
    {
        Player = value;

        yield return null;
    }

    public IEnumerator<DialoguesAndOptions.DialogueSystem> Request()
    {
        throw new NotImplementedException();
    }

    IEnumerator<EntitiesToReset> IRequest<EntitiesToReset>.Request()
    {
        throw new NotImplementedException();
    }

    IEnumerator<bool> IRequest<bool>.Request()
    {
        throw new NotImplementedException();
    }

    IEnumerator<Collider2D> IRequest<Collider2D>.Request()
    {
        throw new NotImplementedException();
    }
}








