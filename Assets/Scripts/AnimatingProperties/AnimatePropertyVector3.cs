using System.Collections;
using UnityEngine;

public class AnimatePropertyVector3 : IAnimatePropertyVector<Vector3>
{

    public enum MovementType
    {
        SLOW,
        MEDIUM,
        FAST
    }

    public IEnumerator AnimMovement(Vector3 initialPos, Vector3 finalPos, float duration)
    {
        Vector3 currentPosition = initialPos;
        while(currentPosition != finalPos)
        {
           
        }

        yield return new WaitForSeconds(0f);
    }

    public Vector3 GetDifference(Vector3 initialValue, Vector3 finalValue)
    {
        return finalValue - initialValue;
    }
}