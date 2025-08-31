using UnityEngine;

public class MovementHelperClass : IOverlapChecker
{
    public override bool OverlapAgainstLayerMaskChecker(Collider2D gameObject, LayerMask colliderLayerMask, float distance) //default is .1f
    {
        return Physics2D.CapsuleCast(gameObject.bounds.center, gameObject.bounds.size, CapsuleDirection2D.Vertical, 0f, Vector2.down, distance, colliderLayerMask);
    }
}
