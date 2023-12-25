using System.Collections;
using UnityEngine;
public class BossScript : AbstractEntity
{
    private GameObject Player;
    private Animator anim;
    private float TimeoverBody = 0f;
    private BoxCollider2D _bC2;
    private bool onTopBossBool = false;
    [SerializeField] GameObject BossDead;
    public override string EntityName { get => m_Name; set => m_Name = value; }
    public override float Health { get => m_health; set => m_health = value; }
    public override float MaxHealth { get => m_maxHealth; set => m_maxHealth = value; }

    private void Awake()
    {
        MaxHealth = 100f;
        Health = MaxHealth;
    }

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        anim = GetComponent<Animator>();
        _bC2 = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player");

        }else
        {
            CheckRotation();

            if (GameObjectCreator.GetDialogueManager().getIsOpen())
            {
                anim.SetBool("walk", false);
            }
            if (onTopBossBool)
            {
                TimeoverBody += Time.deltaTime;
            }

            if (TimeoverBody > .5f)
            {
                _bC2.enabled = false;
                onTopBossBool = false;
                StartCoroutine(TimeElapse());

            }
        }

    }

    IEnumerator TimeElapse()
    {
        yield return new WaitForSeconds(.5f);
        _bC2.enabled = true;
        TimeoverBody = 0f;

    }

    void CheckRotation()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("attack") && !anim.GetCurrentAnimatorStateInfo(0).IsName("attack_02"))
        {
            if (transform.position.x > Player.transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if (transform.position.x < Player.transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword") || collision.CompareTag("Dagger"))
        {
            anim.SetTrigger("damage");
            Health -= 10;

        }
       
        if (Health == 0)
        {
            Vector2 pos = transform.position;
            pos.y = transform.position.y + .5f;
            GameObject dead = Instantiate(BossDead, pos, Quaternion.identity);
            Destroy(gameObject);
            Destroy(dead, 1f);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && onTopBossBool == false)
        {

            onTopBossBool = true;

        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            onTopBossBool = false;

        }

    }

    public override void GameStateHandler(SceneData data)
    {
        throw new System.NotImplementedException();
    }
}
