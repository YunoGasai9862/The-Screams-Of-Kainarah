using UnityEngine;

public class RakashControllerMovement : MonoBehaviour, IReceiver<MovementAnimationPackage, ActionExecuted>
{
    private const float OVER_GROUND = 1.5f;

    private const float SPEED = 4f;

    ActionExecuted IReceiver<MovementAnimationPackage, ActionExecuted>.PerformAction(MovementAnimationPackage value)
    {
        value.Animator.transform.position = Vector3.MoveTowards(value.Animator.transform.position, 
            new Vector3(value.TargetTransform.position.x, value.TargetTransform.position.y - OVER_GROUND, 
            value.TargetTransform.position.z), SPEED * Time.deltaTime);



        return new ActionExecuted { };
    }

    public ActionExecuted CancelAction()
    {
        return new ActionExecuted { };
    }
}