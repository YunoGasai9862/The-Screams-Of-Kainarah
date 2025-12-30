using Assets.Annotations;
using Assets.Scripts.GenericDelegators;
using Assets.Scripts.Models.Reset;
using System.Linq;
using System.Threading;
using UnityEngine;

[Subject(SubjectType = typeof(PlayerAttackStateMachineReset), ContextType = typeof(ResetBundle)]
public class PlayerAttackStateMachineReset : StateMachineBehaviour, IObserver<EntityPoolManager>, ISubject<ResetBundle>
{
    [SerializeField]
    public string resetConfigEntityName;
    private AttackResetConfig AttackResetConfig { get; set; }

    private EntityPoolManagerDelegator EntityPoolManagerDelegator { get; set; }

    private EntityPoolManager EntityPoolManager { get; set; }

    private ResetBundleDelegator ResetBundleDelegator { get; set; }

    private async void OnEnable()   
    {
        ResetBundleDelegator = await Helper.GetDelegator<ResetBundleDelegator>();

        EntityPoolManagerDelegator = await Helper.GetDelegator<EntityPoolManagerDelegator>();

        ResetBundleDelegator.AddToSubjectsDict(name, name, new Subject<ResetBundle>(this, typeof(PlayerAttackStateMachineReset)));

        EntityPoolManagerDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {

            ObserverName = this.name,
            ObserverTag = this.name,
            SubjectType = typeof(EntityPoolManager).ToString()

        }, CancellationToken.None);
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override async public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetBundleDelegator.NotifyObservers(new ResetBundle()
        {
            Animator = animator,
            ResetSystem = new ResetSystem()
            {
                ResetParameters = AttackResetConfig.resetParameters.Select(reset => new Reset()
                {
                    m_key = reset.key,
                    m_val = new Reset.Value()
                    {
                        m_type = reset.type,
                        m_newValue = reset.type == AnimatorControllerParameterType.Int ? 0 : (reset.type == AnimatorControllerParameterType.Float ? 0.0f : (reset.type == AnimatorControllerParameterType.Bool ? false : null))
                    }
                }).ToList(),
                State = ResetState.PARTIAL_RESET
            }

        }, name, CancellationToken.None);
    }

    public void OnNotify(EntityPoolManager data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        EntityPoolManager = data;

        AttackResetConfig = (AttackResetConfig) EntityPoolManager.GetPooledEntity(resetConfigEntityName).FirstOrDefault(entity => entity.Name.Equals(resetConfigEntityName)).Entity;
    }

    public void OnNotifySubject(IObserver<ResetBundle> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        ResetBundleDelegator.CreateAssociation(name, ResetBundleDelegator.GetSubsetSubjectsDictionary(name)[name], observer);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}