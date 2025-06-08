using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static SceneData;
public class RakashManager : AbstractEntity, IObserver<GameState>, IObserver<Player>
{
    private Animator _anim;
    private float _timeoverBody = 0f;
    private BoxCollider2D _bC2;
    private bool _onTopBossBool = false;
    private IReceiver<bool> m_rakashMovementControllerReceiver;
    private Command<bool> m_rakashMovementControllerCommand;

    private IReceiver<bool> m_rakashAttackControllerReceiver;
    private Command<bool> m_rakashAttackControllerCommand;
    private GameState CurrentGameState { get; set; }

    [SerializeField] GameObject bossDead;
    [SerializeField] string[] attackingAnimationNames;
    [SerializeField] GlobalGameStateDelegator gameStateDelegator;
    [SerializeField] PlayerAttributesDelegator playerAttributesDelegator;
    public override string EntityName { get => m_Name; set => m_Name = value; }
    public override float Health { get => m_health; set => m_health = value; }
    public override float MaxHealth { get => m_maxHealth; set => m_maxHealth = value; }
    private Player Player { get; set; }

    public EntityDistanceFromPlayer distanceFromPlayerEvent = new EntityDistanceFromPlayer();

    private void Awake()
    {
        MaxHealth = 100f;
        Health = MaxHealth;
    }

    void Start()
    {
        StartCoroutine(gameStateDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = name,
            ObserverTag = tag,
            SubjectType = typeof(GlobalGameStateManager).ToString()

        }, CancellationToken.None));

        StartCoroutine(playerAttributesDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = name,
            ObserverTag = tag,
            SubjectType = typeof(PlayerAttributesNotifier).ToString()

        }, CancellationToken.None));

        m_rakashMovementControllerReceiver = GetComponent<RakashControllerMovement>();
        m_rakashMovementControllerCommand = new Command<bool>(m_rakashMovementControllerReceiver);

        m_rakashAttackControllerReceiver = GetComponent<RakashAttackController>();
        m_rakashAttackControllerCommand = new Command<bool>(m_rakashAttackControllerReceiver);


        _anim = GetComponent<Animator>();
        _bC2 = GetComponent<BoxCollider2D>();
        SceneSingleton.InsertIntoGameStateHandlerList(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Player == null)
        {
            return;
        }

        CheckRotation();

        if (CurrentGameState.Equals(GameState.DIALOGUE_TAKING_PLACE))
        {
            _anim.SetBool("walk", false);
        }
        if (_onTopBossBool)
        {
            _timeoverBody += Time.deltaTime;
        }

        if (_timeoverBody > .5f)
        {
            _bC2.enabled = false;
            _onTopBossBool = false;
            StartCoroutine(TimeElapse());

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
            if (transform.position.x > Player.Transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            if (transform.position.x < Player.Transform.position.x)
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
        if (await EnemyHittableManager.IsEntityAnAttackObject(collision, SceneSingleton.EnemyHittableObjects))
        {
            _anim.SetTrigger("damage");
            Health -= 10;
        }
       
        if (Health == 0)
        {
            Vector2 pos = transform.position;
            pos.y = transform.position.y + .5f;
            var deadBody =await HandleBossDefeatScenario(pos, bossDead, gameObject);
            await DestroyMultipleGameObjects(new[] { deadBody, gameObject }, 1f);
        }
    }

    private Task<GameObject> HandleBossDefeatScenario(Vector3 position, GameObject prefab, GameObject mainObject)
    {
        GameObject dead = Instantiate(prefab, position, Quaternion.identity);
        return Task.FromResult(dead);   
    }
    private Task DestroyMultipleGameObjects(GameObject[] gameObjects, float destroyInSeconds)
    {
       foreach(var gameObject in gameObjects)
       {
            Destroy(gameObject, destroyInSeconds);
       }
       return Task.CompletedTask;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !_onTopBossBool)
        {
            _onTopBossBool = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            _onTopBossBool = false;
        }

    }
    public override void GameStateHandler(SceneData data)
    {
        ObjectData bossData = new ObjectData(transform.tag, transform.name, transform.position, transform.rotation);

        data.AddToObjectsToPersist(bossData);
    }

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentGameState = data;
    }

    public void OnNotify(Player data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Player = data;
    }
}
