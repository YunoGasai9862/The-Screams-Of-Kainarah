
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class AnimatePropertyColor : IAnimatePropertyColor<Color>
{
    private float TimePassed { get; set; } = 0;
    private UnityAction<Color> _callBack;
    private NotifyColorChangeEvent _notifyColorChangeEvent;
    public AnimatePropertyColor(UnityAction<Color> callBack)
    {
        _callBack = callBack;
        _notifyColorChangeEvent = new NotifyColorChangeEvent();
        _notifyColorChangeEvent.AddListener(_callBack);
    }

    public IEnumerator AnimColor(Color initialColor, Color endColor, float duration)
    {
        //use Color. LERP
        Color difference = GetDifference(initialColor, endColor);
        Color temp = initialColor;
        Color sign;
        Debug.Log($"Difference: {difference}");
        Debug.Log($"Initial: {initialColor}");
        Debug.Log($"End: {endColor}");
        while (TimePassed < duration && temp != endColor)
        {
            //it's working but polish it!!
            TimePassed += Time.deltaTime;
            sign = CalculateSign(difference);
            Color newValue = new Color(initialColor.r + (sign.r * TimePassed), initialColor.g + (sign.g * TimePassed), initialColor.b + (sign.b * TimePassed), initialColor.a + (sign.a * TimePassed));
            Debug.Log((TimePassed, newValue, sign));
            _notifyColorChangeEvent.Invoke(newValue);
            yield return new WaitForSeconds(1f);
        }
    }

    public Color GetDifference(Color initialValue, Color finalValue)
    {
        return new Color(finalValue.r - initialValue.r, finalValue.g - initialValue.g, finalValue.b - initialValue.b, finalValue.a - initialValue.a);
    }
    
    public Color CalculateSign(Color differenceValue)
    {
        Color sign = new Color(0,0,0,0);
        sign.r = differenceValue.r >= 0 ? 1 : -1;
        sign.g = differenceValue.g >= 0 ? 1 : -1;
        sign.b = differenceValue.b >= 0 ? 1 : -1;
        sign.a = differenceValue.a >= 0 ? 1 : -1;
        return sign;
    }
}