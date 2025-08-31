using UnityEngine;

public abstract class IOverlapChecker
{
    public abstract bool OverlapAgainstLayerMaskChecker(Collider2D gameObject, LayerMask colliderLayerMask, float distance);
}

