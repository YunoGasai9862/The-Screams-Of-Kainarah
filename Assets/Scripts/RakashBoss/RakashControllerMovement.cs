using System.Collections.Generic;
using UnityEngine;

public class RakashControllerMovement : MonoBehaviour, IReceiver<MovementAnimationPackage, Vector3>
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

    //FIX THIS - since this is on the same controller, we dont need to return anything i guess
    Vector3 IReceiver<MovementAnimationPackage, Vector3>.PerformAction(MovementAnimationPackage value)
    {
        AnimationUtility.ExecuteAnimation(value.Animation, value.Animator);

        if (value.TargetTransform == null)
        {
            return value.Animator.transform.position;
        }

       return Vector3.MoveTowards(value.Animator.transform.position, 
               new Vector3(value.TargetTransform.position.x, 
               value.TargetTransform.position.y - OVER_GROUND, 
               value.TargetTransform.position.z), SPEED * Time.deltaTime);

    }

    public Vector3 CancelAction()
    {
        return new Vector3 { };
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