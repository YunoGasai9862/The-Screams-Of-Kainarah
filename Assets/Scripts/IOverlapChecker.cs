using UnityEngine;

public abstract class IOverlapChecker
{
    public abstract bool overlapAgainstLayerMaskChecker(ref CapsuleCollider2D gameObject, LayerMask colliderLayerMask, float distance);
}

