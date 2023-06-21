using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IOverlapChecker
{
    public abstract bool overlapAgainstLayerMaskChecker(ref Collider2D gameObject, float size, LayerMask colliderLayerMask);
  
}
