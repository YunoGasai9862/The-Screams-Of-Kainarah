using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
public class AnimateProperty : IAnimateProperty
{

    IEnumerator IAnimateProperty.AnimColor(Color initialColor, Color endColor, float duration)
    {
        Color difference = GetDifference<Color>(initialColor, endColor);
        Debug.Log(difference);
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator IAnimateProperty.AnimMovement(Vector3 initialTransform, Vector3 endTransform, float duration)
    {
        Vector3 difference = GetDifference<Vector3>(initialTransform, endTransform);
        yield return new WaitForSeconds(0.5f);
    }
    public T GetDifference<T>(T initialValue, T finalValue)
    {
        switch (typeof(T))
        {
            case Type t when t == typeof(Color):
                Color begValueC = (Color)Convert.ChangeType(initialValue, typeof(Color));
                Color endValueC = (Color)Convert.ChangeType(finalValue, typeof(Color));
                Color differenceC = new Color(endValueC.r - begValueC.r, endValueC.g - begValueC.g, endValueC.b - begValueC.b, endValueC.a - begValueC.a);
                return (T) Convert.ChangeType(differenceC, typeof(T));

            case Type t when t == typeof(Vector3):
                Vector3 begValueV = (Vector3)Convert.ChangeType(initialValue, typeof(Vector3));
                Vector3 endValueV = (Vector3)Convert.ChangeType(finalValue, typeof(Vector3));
                Vector3 differenceV = new Vector3(endValueV.x - begValueV.x, endValueV.y - begValueV.y, endValueV.z - begValueV.z);
                return (T)Convert.ChangeType(differenceV, typeof(T));


            case Type t when t == typeof(Vector2):
                Vector2 begValueV2 = (Vector2)Convert.ChangeType(initialValue, typeof(Vector3));
                Vector2 endValueV2 = (Vector3)Convert.ChangeType(finalValue, typeof(Vector3));
                Vector2 differenceV2 = new Vector2(endValueV2.x - begValueV2.x, endValueV2.y - begValueV2.y);
                return (T)Convert.ChangeType(differenceV2, typeof(T));

            default:
                break;
        }
        return (T)Convert.ChangeType(null, typeof(T)); 
    }

    public void SetTarget<T>()
    {

    }

}