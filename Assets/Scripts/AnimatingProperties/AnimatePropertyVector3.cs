using System.Collections;
using UnityEngine;

public class AnimatePropertyVector3 : IAnimatePropertyVector<Vector3>
{
    //change this to another class for keeping movement types
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

    public IEnumerator AnimMovement(Vector3 initialPos, Vector3 finalPos, float duration, IAnimateMovementType movementType)
    {
        throw new System.NotImplementedException();
    }
}