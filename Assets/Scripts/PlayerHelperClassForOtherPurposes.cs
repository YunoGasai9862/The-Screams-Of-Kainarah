using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHelperClassForOtherPurposes : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] GameObject DiamondHitEffect;
    [SerializeField] GameObject pickupEffect;
    [SerializeField] Interactable dialogue;
    [SerializeField] TrackingEntities trackingEntities;
    [SerializeField] List<string> pickableItems;

    public static bool AudioPickUp;

    private Animator anim;
    private bool Death = false;
    public static double MAXHEALTH = 100f;
    public static double ENEMYATTACK = 5f;
    [SerializeField] GameObject TeleportTransition;

    public static bool isGrabbing = false;//for the ledge grab script
    private bool once = true;
    private Collider2D collidedObject;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        MAXHEALTH = 100f;

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
            StartCoroutine(dialogue.TriggerDialogue(dialogue.BossDialogue));

        }
        if (FindingObjects.FindObject(gameObject, "Vendor") && once)
        {
            StartCoroutine(dialogue.TriggerDialogue(dialogue.WizardPlayerConvo));
            once = false;
        }


    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") || (collision.collider.CompareTag("Boss") &&
            (collision.collider.transform.root.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack") ||
            collision.collider.transform.root.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack_02"))))
        {
            if (MAXHEALTH == 0)
            {
                Death = true;
                anim.SetBool("Death", true);
            }
            else
            {
                MAXHEALTH -= ENEMYATTACK;
            }

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collidedObject = collision;

        bool pickedUp = didPlayerCollideWithaPickableItem(collision.name);

        if (pickedUp)
        {
            collidedObject.gameObject.SetActive(false); //hides it
        }
        if (collision.CompareTag("Crystal"))
        {
            GameObject DHE = Instantiate(DiamondHitEffect, collision.transform.position, Quaternion.identity);
            AudioPickUp = true;
            Destroy(DHE, 2f);

        }

        if (collision.CompareTag("Health"))
        {

            if (MAXHEALTH < 100)
            {
                MAXHEALTH += 10;
                GameObject temp = Instantiate(pickupEffect, collision.gameObject.transform.position, Quaternion.identity);
                Destroy(collision.gameObject);
                Destroy(temp, 1f);
            }


        }
    }

    private bool didPlayerCollideWithaPickableItem(string collisionObjectName)
    {
        for (int i = 0; i < pickableItems.Count; i++)
        {
            if (collisionObjectName == pickableItems[i])
            {
                return true;
            }
        }

        return false;
    }
    private void pickupEffectInstantiator()
    {

    }
    public Collider2D getColliderObject()
    {
        return collidedObject;
    }

    public bool checkForExistenceOfPortal(SpriteRenderer sr)
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

    IEnumerator WaiterFunction()
    {
        yield return new WaitForSeconds(1f);
    }


}









