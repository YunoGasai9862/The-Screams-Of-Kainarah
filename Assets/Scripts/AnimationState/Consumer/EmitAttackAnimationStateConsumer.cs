using System.Threading.Tasks;
using UnityEngine.Events;

public class EmitAttackAnimationStateConsumer : BaseState<EmitAnimationStateBundle, AttackState>
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
        EmitAnimationAttackStateDelegator.AddToSubjectsDict(typeof(EmitAttackAnimationStateConsumer).ToString(), name, new Subject<IObserver<GenericStateBundle<EmitAnimationStateBundle, AttackState>>>());

        EmitAnimationAttackStateDelegator.GetSubsetSubjectsDictionary(typeof(EmitAttackAnimationStateConsumer).ToString())[name].SetSubject(this);

        return Task.CompletedTask;
    }

    protected override async Task<BaseDelegator<GenericStateBundle<EmitAnimationStateBundle, AttackState>>> GetDelegator()
    { 
        return EmitAnimationAttackStateDelegator;
    }

    protected override async Task<UnityEvent<GenericStateBundle<EmitAnimationStateBundle, AttackState>>> GetEvent()
    {
        return EmitAnimationStateEvent.GetInstance();
    }

    protected override GenericStateBundle<EmitAnimationStateBundle, AttackState> GetInitialState()
    {
        return new GenericStateBundle<EmitAnimationStateBundle, AttackState>()
        {
            StateBundle = new EmitAnimationStateBundle()
            {
                IsRunning = false
            }
        };
    }
}