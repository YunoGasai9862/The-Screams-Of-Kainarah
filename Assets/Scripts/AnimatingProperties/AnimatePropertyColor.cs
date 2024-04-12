
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class AnimatePropertyColor : IAnimatePropertyColor<Color>
{
    private float TimePassed { get; set; } = 0;
    private UnityAction<Color> _callBack;
    private NotifyColorChangeEvent _notifyColorChangeEvent = new NotifyColorChangeEvent();

    public AnimatePropertyColor(UnityAction<Color> callBack)
    {
        _callBack = callBack;
        _notifyColorChangeEvent.AddListener(_callBack);
    }

    public IEnumerator AnimColor(Color initialColor, Color endColor, float duration)
    {
        //use Color. LERP
        Color difference = GetDifference(initialColor, endColor);
        while(TimePassed < duration)
        {
            TimePassed += Time.deltaTime;
            //find a way to calculate differences + modify the original color/values via SetTarget??
             difference = new Color(initialColor.r + TimePassed, initialColor.g + TimePassed, initialColor.b, initialColor.a + TimePassed);
            Debug.Log(difference);
            _notifyColorChangeEvent.Invoke(difference);
            
        }
        
        yield return new WaitForSeconds(0.5f);
    }

    public Color GetDifference(Color initialValue, Color finalValue)
    {
        return new Color(finalValue.r - initialValue.r, finalValue.g - initialValue.g, finalValue.b - initialValue.b, finalValue.a - initialValue.a);
    }
    
    public Vector3 GetSign(Color differenceValue)
    {
        //x ->r , y->g, z->b
        Vector3 sign = new Vector3(0,0,0);
        sign.x = differenceValue.r > 0 ? 1 : -1;
        sign.y = differenceValue.g > 0 ? 1 : -1;
        sign.z = differenceValue.b > 0 ? 1 : -1;
        return sign;
    }
}