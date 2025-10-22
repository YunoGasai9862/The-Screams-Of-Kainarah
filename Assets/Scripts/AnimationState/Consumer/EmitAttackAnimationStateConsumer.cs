using System.Threading.Tasks;
using UnityEngine.Events;

public class EmitAttackAnimationStateConsumer : BaseState<EmitAnimationStateBundle<bool>, AttackState>
{
    private EmitAnimationAttackStateDelegator EmitAnimationAttackStateDelegator { get; set; }

    private EmitAttackAnimationStateEvent EmitAnimationStateEvent { get; set; }

    protected override async Task AddDelegator()
    {
        EmitAnimationAttackStateDelegator = await Helper.GetDelegator<EmitAnimationAttackStateDelegator>();
    }

    protected override async Task AddEvent()
    {
        EmitAnimationStateEvent = await Helper.GetCustomEvent<EmitAttackAnimationStateEvent>();
    }

    protected override Task AddSubject()
    {
        EmitAnimationAttackStateDelegator.AddToSubjectsDict(typeof(EmitAttackAnimationStateConsumer).ToString(), name, new Subject<IObserver<GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState>>>());

        EmitAnimationAttackStateDelegator.GetSubsetSubjectsDictionary(typeof(EmitAttackAnimationStateConsumer).ToString())[name].SetSubject(this);

        return Task.CompletedTask;
    }

    protected override async Task<BaseDelegator<GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState>>> GetDelegator()
    { 
        return EmitAnimationAttackStateDelegator;
    }

    protected override async Task<UnityEvent<GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState>>> GetEvent()
    {
        return EmitAnimationStateEvent.GetInstance();
    }

    protected override GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState> GetInitialState()
    {
        return new GenericStateBundle<EmitAnimationStateBundle<bool>, AttackState>()
        {
            StateBundle = new EmitAnimationStateBundle<bool>()
            {
                CurrentAnimation = new EmitAnimationStateBundle<bool>.CurrentAnimationInfo<bool>()
                {
                    CurrentValue = false,
                }
            }
        };
    }
}