
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class AnimatePropertyColor : IAnimatePropertyColor<Color>
{
    private float _timePassed { get; set; } = 0;
    private UnityAction<Color> callBack;

    public AnimatePropertyColor(UnityAction<Color> callBack)
    {
        this.callBack = callBack;
    }

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

    public Color GetDifference(Color initialValue, Color finalValue)
    {
        return new Color(finalValue.r - initialValue.r, finalValue.g - initialValue.g, finalValue.b - initialValue.b, finalValue.a - initialValue.a);

    }
}