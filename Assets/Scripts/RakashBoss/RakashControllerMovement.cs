using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RakashControllerMovement : MonoBehaviour, IReceiver<MovementActionDelegatePackage, Task<ActionExecuted>>
{
    private const float OVER_GROUND = 1.5f;

    private const float SPEED = 4f;

    private AnimationUtility AnimationUtility { get; set; }

    private List<RakashMovement> Movement { get; set; }

    public void Start()
    {
        AnimationUtility = new AnimationUtility();

        Movement = new List<RakashMovement>
        { 
            RakashMovement.IDLE,
            RakashMovement.WALK
        };
    }

    Task<ActionExecuted> IReceiver<MovementActionDelegatePackage, Task<ActionExecuted>>.PerformAction(MovementActionDelegatePackage value)
    {

        AnimationUtility.ExecuteAnimation(value.Animation, value.Animator);

        if (value.TargetTransform == null)
        {
            return Task.FromResult(new ActionExecuted());
        }

        value.MainEntityTransform.position = Vector3.MoveTowards(value.MainEntityTransform.position, 
               new Vector3(value.TargetTransform.position.x, 
               value.TargetTransform.position.y - OVER_GROUND, 
               value.TargetTransform.position.z), SPEED * Time.deltaTime);

        return Task.FromResult(new ActionExecuted());

    }

    public Task<ActionExecuted> CancelAction()
    {
        return Task.FromResult(new ActionExecuted { });
    }

    public async void Rotate(List<RakashMovement> movementAnimations, Transform playerTransform, AnimatorStateInfo animatorStateInfo)
    {
        foreach(RakashMovement movementAnimation in movementAnimations)
        {
            if (animatorStateInfo.IsName(await AnimationUtility.ResolveAnimationName(movementAnimation)))
            {
                transform.rotation = transform.position.x > playerTransform.position.x ? transform.rotation = Quaternion.Euler(0, 0, 0) : transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }
}