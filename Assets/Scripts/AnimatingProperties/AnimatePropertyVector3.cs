using System.Collections;
using UnityEngine;

public class AnimatePropertyVector3 : IAnimatePropertyVector<Vector3>
{
    public IEnumerator AnimMovement(Vector3 initialPos, Vector3 finalPos, float duration)
    {
        return null;
    }

    public Vector3 GetDifference(Vector3 initialValue, Vector3 finalValue)
    {
        return Vector3.zero;
    }
}