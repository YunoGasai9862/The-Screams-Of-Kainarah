using GlobalAccessAndGameHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHelperClassForOtherPurposes : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] string InteractableTag;
    [SerializeField] GameObject TeleportTransition;
    private PlayerObserverListener playerObserverListener;

    private Animator anim;
    private bool Death = false;
    private float ENEMYATTACK = 5f;
    public static bool isGrabbing = false;//for the ledge grab script
    private bool once = true;
    private bool pickedUp;
    private PickableItemsClass _pickableItems;
    private Interactable dialogue;
    private void Awake()
    {
        playerObserverListener = FindFirstObjectByType<PlayerObserverListener>();

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
    private void FixedUpdate()
    {
        if (checkForExistenceOfPortal(sr))
        {

            //Instantiate(TeleportTransition, transform.position, Quaternion.identity);
            StartCoroutine(WaiterFunction());
            GameStateManager.ChangeLevel(SceneManager.GetActiveScene().buildIndex);
        }



        if (FindingObjects.FindObject(gameObject, "Boss"))
        {
           // StartCoroutine(dialogue.TriggerDialogue(dialogue.bossDialogue));

        }
        if (FindingObjects.FindObject(gameObject, "Vendor") && once)
        {
           // StartCoroutine(dialogue.TriggerDialogue(dialogue.wizardPlayerConvo));
           // once = false;
        }


    }
    private void OnCollisionEnter2D(Collision2D collision) //FIX THIS TOO
    {
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

            await playerObserverListener.ListenerDelegator<bool>(PlayerObserverListenerHelper.BoolSubjects, shouldMusicBePlayed);
        }

         await playerObserverListener.ListenerDelegator<Collider2D>(PlayerObserverListenerHelper.ColliderSubjects, collision);
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


}








