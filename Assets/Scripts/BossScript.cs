using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using static SceneData;
public class BossScript : AbstractEntity
{
    private GameObject _player;
    private Animator _anim;
    private float _timeoverBody = 0f;
    private BoxCollider2D _bC2;
    private bool onTopBossBool = false;
    [SerializeField] GameObject bossDead;
    [SerializeField] string[] attackingAnimationNames;
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
        _player = GameObject.FindWithTag("Player");
        _anim = GetComponent<Animator>();
        _bC2 = GetComponent<BoxCollider2D>();
        GameObjectCreator.InsertIntoGameStateHandlerList(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player");

        }else
        {
            CheckRotation();

            if (GameObjectCreator.GetDialogueManager().getIsOpen())
            {
                _anim.SetBool("walk", false);
            }
            if (onTopBossBool)
            {
                _timeoverBody += Time.deltaTime;
            }

            if (_timeoverBody > .5f)
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
        _timeoverBody = 0f;

    }

    public async void CheckRotation()
    {
        if (await IsNotOneOfTheAttackingAnimations(attackingAnimationNames, _anim))
        {
            if (transform.position.x > _player.transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if (transform.position.x < _player.transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    private Task<bool> IsNotOneOfTheAttackingAnimations(string[] animationNames, Animator anim)
    {
        bool result = true;
        foreach (string animationName in animationNames)
        {
            result = result && !anim.GetCurrentAnimatorStateInfo(0).IsName(animationName);
        }
        return Task.FromResult(result);
    }

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (await EnemyHittableManager.isEntityAnAttackObject(collision, GameObjectCreator.EnemyHittableObjects))
        {
            _anim.SetTrigger("damage");
            Health -= 10;
        }
       
        if (Health == 0)
        {
            Vector2 pos = transform.position;
            pos.y = transform.position.y + .5f;
            GameObject dead = Instantiate(bossDead, pos, Quaternion.identity);
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
        ObjectData bossData = new ObjectData(transform.tag, transform.name, transform.position, transform.rotation);
        data.AddToObjectsToPersist(bossData);
    }
}
