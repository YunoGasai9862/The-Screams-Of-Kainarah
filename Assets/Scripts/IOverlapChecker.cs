using UnityEngine;

public abstract class IOverlapChecker
{
    public abstract bool OverlapAgainstLayerMaskChecker(ref CapsuleCollider2D gameObject, LayerMask colliderLayerMask, float distance);
}

