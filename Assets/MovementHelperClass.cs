using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHelperClass : IOverlapChecker
{
    public override bool overlapAgainstLayerMaskChecker(ref BoxCollider2D gameObject, LayerMask colliderLayerMask) //default is .2f
    {
        return Physics2D.BoxCast(gameObject.bounds.center, gameObject.bounds.size, 0f, Vector2.down, .1f, colliderLayerMask);
    }

  
}
