using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
public class AnimateProperty : IAnimateProperty
{
    private float _timePassed { get; set; } = 0;
    public IEnumerator AnimColor(Color initialColor, Color endColor, float duration)
    {
        //use Color. LERP
        Color difference = GetDifference(initialColor, endColor);
        while(_timePassed < duration)
        {
            _timePassed += Time.deltaTime;
            //find a way to calculate differences + modify the original color/values via SetTarget??
            difference = new Color(initialColor.r + _timePassed, initialColor.g + _timePassed, initialColor.b, initialColor.a + _timePassed);
            Debug.Log(difference);
        }
        
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator AnimMovement(Vector3 initialPos, Vector3 finalPos, float duration)
    {
        yield return new WaitForSeconds(0.5f);
    }
    public IEnumerator AnimMovement(Vector2 initialPos, Vector2 finalPos, float duration)
    {
        Vector2 difference = GetDifference(initialPos, finalPos);
        yield return new WaitForSeconds(0.5f);
    }
    private Color GetDifference(Color initialValue, Color finalValue)
    {
        return new Color(finalValue.r - initialValue.r, finalValue.g - initialValue.g, finalValue.b - initialValue.b, finalValue.a - initialValue.a);
    }

    private Vector3 GetDifference(Vector3 initialPos, Vector3 finalPos)
    {
        return new Vector3(finalPos.x - initialPos.x, finalPos.y - initialPos.y, finalPos.z - initialPos.z);
    }

    private Vector2 GetDifference(Vector2 initialPos, Vector2 finalPos)
    {
        return new Vector2(finalPos.x - initialPos.x, finalPos.y - initialPos.y);
    }

    public void SetTarget<T>(ref T property)
    {

    }

}