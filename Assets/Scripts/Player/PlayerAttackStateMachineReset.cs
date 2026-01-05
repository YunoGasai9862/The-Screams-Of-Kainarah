using Assets.Annotations;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Models.Reset;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

[Subject(SubjectType = typeof(PlayerAttackStateMachineReset), ContextType = typeof(ResetBundle))]
[Observer(SubjectType = typeof(EntityPoolManager), ObserverType = typeof(PlayerAttackStateMachineReset), DataType = typeof(EntityPoolManager))]
public class PlayerAttackStateMachineReset : StateMachineBehaviour, INotify<EntityPoolManager>, IRequest<ResetBundle>
{
    [SerializeField]
    public string resetConfigEntityName;
    private AttackResetConfig AttackResetConfig { get; set; }

    private EntityPoolManager EntityPoolManager { get; set; }

    private Delegator Delegator { get; set; }

    private ResetBundle CurrentResetBundle { get; set; }

    private async void OnEnable()   
    {

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
        CurrentResetBundle = new ResetBundle()
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

        };

        Delegator.NotifyObserver(new Context<ResetBundle>()
        {
            Data = CurrentResetBundle
        }, CancellationToken.None);
    }


    public Task Notify(EntityPoolManager value)
    {
        EntityPoolManager = value;

        AttackResetConfig = (AttackResetConfig)EntityPoolManager.GetPooledEntity(resetConfigEntityName).FirstOrDefault(entity => entity.Name.Equals(resetConfigEntityName)).Entity;

        return Task.CompletedTask;
    }

    public Task<ResetBundle> Request()
    {
        return Task.FromResult(CurrentResetBundle);
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