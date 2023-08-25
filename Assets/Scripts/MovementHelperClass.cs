using UnityEngine;

public class MovementHelperClass : IOverlapChecker
{
    public override bool overlapAgainstLayerMaskChecker(ref CapsuleCollider2D gameObject, LayerMask colliderLayerMask) //default is .1f
    {
        return Physics2D.CapsuleCast(gameObject.bounds.center, gameObject.bounds.size, CapsuleDirection2D.Vertical, 0f, Vector2.down, .1f, colliderLayerMask);
    }

    public bool overlapAgainstLayerMaskChecker(ref BoxCollider2D gameObject, LayerMask colliderLayerMask)
    {
        return Physics2D.CapsuleCast(gameObject.bounds.center, gameObject.bounds.size, CapsuleDirection2D.Vertical, 0f, Vector2.down, .1f, colliderLayerMask);

    }


}
