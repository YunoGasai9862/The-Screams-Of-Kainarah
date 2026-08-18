using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using Assets.Scripts.Models.Reset;
using Assets.Scripts.BaseScene;
using System.Collections;
using System.Linq;
using UnityEngine;

[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(PlayerAttackStateMachineReset), ContextType = typeof(ResetBundle))]
[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(EntityPoolManager), EntityType = typeof(PlayerAttackStateMachineReset), ContextType = typeof(EntityPoolManager))]
public class PlayerAttackStateMachineReset : StateMachineScene, INotify<EntityPoolManager>, IRequest<ResetBundle>
{
    [SerializeField]
    public string resetConfigEntityName;
    private AttackResetConfig AttackResetConfig { get; set; }

    private EntityPoolManager EntityPoolManager { get; set; }

    private ResetBundle CurrentResetBundle { get; set; }

    private SceneUtils SceneUtils { get; set; }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public async void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (SceneUtils == null)
        {
            SceneUtils = (await (await GetBaseScene()).GetSceneUtilsAsync());

            Debug.Log($"SceneUtils in the PlayerAttackStateMachineReset: {SceneUtils}");

            SceneUtils.NotifySubject(new ObserverContext<EntityPoolManager>()
            {
                EntityType = typeof(PlayerActionRelayer),
                SubjectType = typeof(EntityPoolManager)

            }, this);
        }
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

        SceneUtils.NotifyObservers(new SubjectContext<ResetBundle> { Data = CurrentResetBundle, EntityType = typeof(PlayerAttackStateMachineReset) }, this);
    }


    public IEnumerator Notify(EntityPoolManager value)
    {
        EntityPoolManager = value;

        AttackResetConfig = (AttackResetConfig)EntityPoolManager.GetPooledEntity(resetConfigEntityName).FirstOrDefault(entity => entity.Name.Equals(resetConfigEntityName)).Entity;

        yield return null;
    }

    public IEnumerator Request()
    {
        SceneUtils.NotifyObservers(new SubjectContext<ResetBundle> { Data = CurrentResetBundle, EntityType = typeof(PlayerAttackStateMachineReset) }, this);

        yield return null;
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