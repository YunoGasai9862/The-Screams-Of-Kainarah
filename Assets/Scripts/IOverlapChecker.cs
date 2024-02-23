using UnityEngine;

public abstract class IOverlapChecker
{
    public abstract bool OverlapAgainstLayerMaskChecker(ref Collider2D gameObject, LayerMask colliderLayerMask, float distance);
}

