using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DialogueEntityScriptableObject;
using static GameObjectCreator;

public class PlayerHelperClassForOtherPurposes : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] string InteractableTag;
    [SerializeField] GameObject TeleportTransition;


    private Animator anim;
    private bool Death = false;
    private float ENEMYATTACK = 5f;
    public static bool isGrabbing = false;//for the ledge grab script
    private bool pickedUp;
    private PickableItemsClass _pickableItems;
    private Interactable dialogue;
    private SemaphoreSlim _semaphoreSlim;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;
    private void Awake()
    {
        _semaphoreSlim = new SemaphoreSlim(1);

        _cancellationTokenSource = new CancellationTokenSource();

        _cancellationToken = _cancellationTokenSource.Token;

        _pickableItems= GameObject.FindWithTag("PickableItemsManager").GetComponent<PickableItemsClass>();

        sr = GetComponent<SpriteRenderer>();

        anim = GetComponent<Animator>();

        dialogue= GameObject.FindWithTag(InteractableTag).GetComponent<Interactable>(); 

    }

    private void Update()
    {
        if (dialogue != null)
        {
            dialogue = GameObject.FindWithTag(InteractableTag).GetComponent<Interactable>();
        }
    }
    private async void FixedUpdate()
    {
        if (checkForExistenceOfPortal(sr))
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
        /**
        if (collision.collider.CompareTag("Enemy") || (collision.collider.CompareTag("Boss") &&
            (collision.collider.transform.root.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack") ||
            collision.collider.transform.root.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack_02"))))
        {
            if (HealthManager.getPlayerHealth == 0)
            {
                Death = true;
                anim.SetBool("Death", true);
            }
            else
            {
                HealthManager.getPlayerHealth -= ENEMYATTACK;
            }

        }
        **/

        if (await CanPlayerBeAttacked(PlayerHittableItemScriptableObjectFetch, collision.collider))
        {
            if (HealthManager.getPlayerHealth == 0)
            {
                Death = true;
                anim.SetBool("Death", true);
            }
            else
            {
                HealthManager.getPlayerHealth -= ENEMYATTACK;
            }
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
    }


    private bool checkForExistenceOfPortal(SpriteRenderer sr)
    {
        RaycastHit hit; //using 3D raycast because of 3D object, portal
        Vector2 pos = transform.position;
        if (sr.flipX)
        {
            pos.x = transform.position.x - 1f;
            Debug.DrawRay(pos, -transform.right * 1, Color.red);
            Physics.Raycast(transform.position, -transform.right, out hit, 1f);



        }
        else
        {
            pos.x = transform.position.x + 1f;

            Debug.DrawRay(transform.position, transform.right * 1, Color.red);

            Physics.Raycast(transform.position, -transform.right, out hit, 1f);


        }


        return hit.collider != null && hit.collider.isTrigger && hit.collider.CompareTag("Portal");
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








