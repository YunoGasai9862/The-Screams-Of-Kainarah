using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IOverlapChecker
{
    public abstract bool overlapAgainstLayerMaskChecker(ref BoxCollider2D gameObject, LayerMask colliderLayerMask);

  
}
