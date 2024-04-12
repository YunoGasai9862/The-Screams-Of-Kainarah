using System.Collections;
using UnityEngine;

public class AnimatePropertyVector2 : IAnimatePropertyVector<Vector2>
{
    public IEnumerator AnimMovement(Vector2 initialPos, Vector2 finalPos, float duration)
    {
        return null;
    }

    public Vector2 GetDifference(Vector2 initialValue, Vector2 finalValue)
    {
        return Vector2.zero;
    }
}
