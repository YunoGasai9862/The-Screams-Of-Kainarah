
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerSystem : MonoBehaviour, ISubject<IObserver<Health>>, ISubject<IObserver<PlayerSystem>>
{
    private HealthDelegator HealthDelegator { get; set; }

    private PlayerSystemDelegator PlayerSystemDelegator { get; set; }

    public bool IS_JUMPING { get; private set; }
    public bool IS_ATTACKING { get; private set; }
    public bool IS_SLIDING { get; private set; }
    public bool IS_RUNNING { get; private set; }
    public bool IS_WALKING { get; private set; }
    public bool IS_GRABBING { get; private set; }
    public bool IS_FALLING { get; private set; }

    private Health PlayerHealth { get; set; }

    public PlayerWalkVariableEvent walkVariableEvent = new PlayerWalkVariableEvent();
    public PlayerRunVariableEvent runVariableEvent = new PlayerRunVariableEvent();
    public PlayerSlideVariableEvent slideVariableEvent = new PlayerSlideVariableEvent();
    public PlayerGrabVariableEvent grabVariableEvent = new PlayerGrabVariableEvent();
    public PlayerAttackVariableEvent attackVariableEvent = new PlayerAttackVariableEvent();
    public PlayerJumpVariableEvent jumpVariableEvent = new PlayerJumpVariableEvent();
    public PlayerFallVariableEvent fallVariableEvent = new PlayerFallVariableEvent();

    private void Awake()
    {
        PlayerHealth = new Health()
        {
            MaxHealth = 100f,
            CurrentHealth = 100f,
            EntityName = name
        };

        HealthDelegator = Helper.GetDelegator<HealthDelegator>();

        PlayerSystemDelegator = Helper.GetDelegator<PlayerSystemDelegator>();
    }

    private void Start()
    {
        walkVariableEvent.AddListener(SetWalkVariableState);
        runVariableEvent.AddListener(SetRunVariableState);
        slideVariableEvent.AddListener(SetSlideVariableState);
        grabVariableEvent.AddListener(SetGrabVariableState);
        attackVariableEvent.AddListener(SetAttackVariableState);
        jumpVariableEvent.AddListener(SetJumpVariableState);
        fallVariableEvent.AddListener(SetFallVariableState);

        HealthDelegator.AddToSubjectsDict(typeof(PlayerSystem).ToString(), name, new Subject<IObserver<Health>>());
        HealthDelegator.GetSubsetSubjectsDictionary(typeof(PlayerSystem).ToString())[name].SetSubject(this);

        PlayerSystemDelegator.AddToSubjectsDict(typeof(PlayerSystem).ToString(), name, new Subject<IObserver<PlayerSystem>>());
        PlayerSystemDelegator.GetSubsetSubjectsDictionary(typeof(PlayerSystem).ToString())[name].SetSubject(this);

    }
    private void SetAttackVariableState(bool variableState)
    {
        IS_ATTACKING = variableState;
    }
    private void SetJumpVariableState(bool variableState)
    {
        IS_JUMPING = variableState;
    }
    private void SetSlideVariableState(bool variableState)
    {
        IS_SLIDING = variableState;
    }
    private void SetWalkVariableState(bool variableState)
    {
        IS_WALKING = variableState;
    }
    private void SetGrabVariableState(bool variableState)
    {
        IS_GRABBING = variableState;
    }
    private void SetRunVariableState(bool variableState)
    {
        IS_RUNNING = variableState;
    }
    private void SetFallVariableState(bool variableState)
    {
        IS_FALLING = variableState;
    }

    private Task<List<string>> GetPlayerAnimationsList(Animator anim)
    {
        var animationController = anim.runtimeAnimatorController;

        List<string> animationNames = new List<string>();
        
        foreach(AnimationClip clip in animationController.animationClips)
        {
            animationNames.Add(clip.name);
        }

        return Task.FromResult(animationNames);
    }
    public async Task<string> GetAnimationName(Animator anim, string search) 
    {
        List<string> animationNames = await GetPlayerAnimationsList(anim);

        return animationNames.Where(e => e.Equals(search) || e.Contains(search)).FirstOrDefault();
    }

    public Task<int> PlayerFlipped(Transform transform)
    {
        return transform.localScale.x < 0 ? Task.FromResult(-1) : Task.FromResult(1);
    }

    public void OnNotifySubject(IObserver<Health> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(HealthDelegator.NotifyObserver(data, PlayerHealth, new NotificationContext()
        {
            SubjectType = typeof(PlayerSystem).ToString()

        }, CancellationToken.None));
    }

    public void OnNotifySubject(IObserver<PlayerSystem> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(PlayerSystemDelegator.NotifyObserver(data, this, new NotificationContext()
        {
            SubjectType = typeof(PlayerSystem).ToString()

        }, CancellationToken.None));
    }
}
