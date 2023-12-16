using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DialogueEntityScriptableObject;
using static GameObjectCreator;

public class PlayerActionRelayer : AbstractEntity
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] string InteractableTag;
    [SerializeField] GameObject TeleportTransition;
    [SerializeField] string[] checkpointTags;

    private Animator anim;
    private float ENEMYATTACK = 5f;
    public static bool isGrabbing = false;//for the ledge grab script
    private bool pickedUp;
    private PickableItemsClass _pickableItems;
    private Interactable dialogue;
    private SemaphoreSlim _semaphoreSlim;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;
    public override string EntityName { get => m_Name; set => m_Name = value; }
    public override float Health { get => m_health; set => m_health = value; }
    public override float MaxHealth { get => m_maxHealth; set => m_maxHealth = value; }

    private void Awake()
    {
        MaxHealth = 100f;
        Health = MaxHealth;

        DontDestroyOnLoad(this);

        _semaphoreSlim = new SemaphoreSlim(1);

        _cancellationTokenSource = new CancellationTokenSource();

        _cancellationToken = _cancellationTokenSource.Token;

        _pickableItems= GameObject.FindWithTag("PickableItemsManager").GetComponent<PickableItemsClass>();

        sr = GetComponent<SpriteRenderer>();

        anim = GetComponent<Animator>();

        dialogue= GameObject.FindWithTag(InteractableTag).GetComponent<Interactable>(); 

    }

    private async void Update()
    {
        if (dialogue != null)
        {
            dialogue = GameObject.FindWithTag(InteractableTag).GetComponent<Interactable>();
        }
        _semaphoreSlim.Wait();

        if (await IsPlayerDead(Health))
        {
            anim.SetBool("Death", true);
            await Task.Delay(TimeSpan.FromSeconds(0.1f));
            await GetPlayerObserverListenerObject().ListenerDelegator<EntitiesToReset>(PlayerObserverListenerHelper.EntitiesToReset, EntitiesToResetScriptableObjectFetch);
            await GetPlayerObserverListenerObject().ListenerDelegator<GameObject>(PlayerObserverListenerHelper.MainPlayerListener, gameObject);
            _semaphoreSlim.Release();
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

        await _semaphoreSlim.WaitAsync();

        (bool inSight, DialogueEntity entity) = await isGameObjectInSightForDialogueTrigger(DialogueEntityScriptableObjectFetch, _cancellationToken, _semaphoreSlim); //use of tuple return

        if (inSight && entity != null)
        {
            await GetPlayerObserverListenerObject().ListenerDelegator<DialogueEntity>(PlayerObserverListenerHelper.DialogueEntites, entity); //test this out
        }

    }
    private async Task<(bool, DialogueEntity)> isGameObjectInSightForDialogueTrigger(DialogueEntityScriptableObject scriptableObject, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim)
    {
        bool inSight = false;
        DialogueEntity dialogueEntity = null;

        foreach (var item in scriptableObject.entities)
        {
            await Task.Delay(TimeSpan.FromSeconds(.1f));

            if(!cancellationToken.IsCancellationRequested && FindingObjects.CastRayToFindObject(gameObject, item.entity.tag))
            {
                inSight = true;
                dialogueEntity = item;
                break;
            }
        }
        semaphoreSlim.Release();
        return (inSight, dialogueEntity);
    }
    private async void OnCollisionEnter2D(Collision2D collision) //FIX THIS TOO
    {
       
        if (await CanPlayerBeAttacked(PlayerHittableItemScriptableObjectFetch, collision.collider))
        {
            Health -= ENEMYATTACK;
        }
    }

    private Task<bool> CanPlayerBeAttacked(PlayerHittableItemsScriptableObject scriptableObject, Collider2D collider)
    {
        foreach (var item in scriptableObject.colliderItems)
        {
            if (item.collider.tag == collider.tag && !item.isItBasedOnAnimationName)
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
        pickedUp = _pickableItems.didPlayerCollideWithaPickableItem(collision.tag);

        if (pickedUp)
        {
            bool shouldbedisabled = _pickableItems.shouldThisItemBeDisabled(collision.tag);

            if (shouldbedisabled)
                collision.gameObject.SetActive(false);

            bool shouldMusicBePlayed = true;

            await GetPlayerObserverListenerObject().ListenerDelegator<bool>(PlayerObserverListenerHelper.BoolSubjects, shouldMusicBePlayed);
        }

         await GetPlayerObserverListenerObject().ListenerDelegator<Collider2D>(PlayerObserverListenerHelper.ColliderSubjects, collision);

        if(await GetOneOfTheCheckPoints(collision.tag, checkpointTags))
        {
           //call checkpoint replacement
        }

    }

    private async Task<bool> GetCheckPointFromScriptableObject(CheckPoints checkpointsScriptableObject, string tag)
    {
        return true;
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

        int sign = sr.flipX ? -1 : 1;

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

}








