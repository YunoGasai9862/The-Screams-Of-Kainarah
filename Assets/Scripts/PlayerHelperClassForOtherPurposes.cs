using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerHelperClassForOtherPurposes : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] GameObject DiamondHitEffect;
    [SerializeField] GameObject pickupEffect;
    [SerializeField] Interactable dialogue;
    [SerializeField] TrackingEntities trackingEntities;

    public static bool AudioPickUp;

    private Animator anim;
    private Rigidbody2D rb;
    private bool Death = false;
    public static double MAXHEALTH = 100f;
    public static double ENEMYATTACK = 5f;
    [SerializeField] GameObject TeleportTransition;

    public static bool isGrabbing = false;//for the ledge grab script
    private bool once = true;
    private Collider2D collidedObject;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        MAXHEALTH = 100f;

    }
    void Update()
    {
        if (!GameObjectCreator.getDialogueManager().getIsOpen())
        {

            if (Death)
            {
                rb.bodyType = RigidbodyType2D.Static;
            }


            if (rb.velocity.y < -15f) //freefalling into an abyss. Not a good solution, i know
            {
                GameStateManager.RestartGame();
            }

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
        collidedObject.gameObject.SetActive(false); //hides it
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
        if (hit.collider != null)

            Debug.Log(hit.collider.name);


        if (hit.collider != null && hit.collider.isTrigger && hit.collider.CompareTag("Portal"))
        {
            return true;
        }

        return false;


    }

    IEnumerator WaiterFunction()
    {
        yield return new WaitForSeconds(1f);
    }


}









