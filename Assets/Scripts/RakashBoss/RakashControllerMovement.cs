using UnityEngine;

public class RakashControllerMovement : MonoBehaviour, IReceiver<MovementAnimationPackage, Vector3>
{
    private const float OVER_GROUND = 1.5f;

    private const float SPEED = 4f;

    private AnimationUtility AnimationUtility { get; set; }

    public void Start()
    {
        AnimationUtility = new AnimationUtility();
    }

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
}