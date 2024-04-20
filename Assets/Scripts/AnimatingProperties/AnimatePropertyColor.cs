
using System;
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

        Color difference = GetDifference(initialColor, endColor);
        Color newValue = new Color(0, 0, 0, 0);
        Color sign = CalculateSign(difference);
        Debug.Log(sign.a);
        Color chunkSize = CalculateChunkSize(duration, difference);
        Color tempChunkSize = chunkSize;
        while (CheckIfUpdatedColorValueIsWithinRange(newValue, endColor, difference))
        {
            TimePassed += Time.deltaTime;
            float progress = Mathf.Clamp(TimePassed, 0, duration);
            Debug.Log($"Progress: {progress}");
            newValue = new Color(initialColor.r + (sign.r * tempChunkSize.r * TimePassed), initialColor.g + (sign.g * tempChunkSize.g * TimePassed), initialColor.b + (sign.b * tempChunkSize.b * TimePassed), initialColor.a + (sign.a * tempChunkSize.a * TimePassed));
            tempChunkSize = tempChunkSize + chunkSize;
            Debug.Log($"New Chunk Size = {chunkSize} TimePassed = {TimePassed}");
            _notifyColorChangeEvent.Invoke(newValue);
            yield return null;
        }

        TimePassed = 0;
    }

    public Color GetDifference(Color initialValue, Color finalValue)
    {
        return new Color(finalValue.r - initialValue.r, finalValue.g - initialValue.g, finalValue.b - initialValue.b, finalValue.a - initialValue.a);
    }

    public Color CalculateSign(Color differenceValue)
    {
        Color sign = new Color(0, 0, 0, 0);
        sign.r = differenceValue.r == 0 ? 0 : differenceValue.r > 0 ? 1 : -1;
        sign.g = differenceValue.g == 0 ? 0 : differenceValue.g > 0 ? 1 : -1;
        sign.b = differenceValue.b == 0 ? 0 : differenceValue.b > 0 ? 1 : -1;
        sign.a = differenceValue.a == 0 ? 0 : differenceValue.a > 0 ? 1 : -1;
        return sign;
    }

    public bool CheckIfUpdatedColorValueIsWithinRange(Color newValue, Color endValue, Color difference)
    {
        bool rCheck = difference.r >= 0 ? newValue.r <= endValue.r : newValue.r >= endValue.r;
        bool gCheck = difference.g >= 0 ? newValue.g <= endValue.g : newValue.g >= endValue.g;
        bool bCheck = difference.b >= 0 ? newValue.b <= endValue.b : newValue.b >= endValue.b;
        bool aCheck = difference.a >= 0 ? newValue.a <= endValue.a : newValue.a >= endValue.a;
        return rCheck && gCheck && bCheck && aCheck;
    }

    public Color CalculateChunkSize(double durationInSeconds, Color difference)
    {
        TimeSpan duration = TimeSpan.FromMilliseconds(durationInSeconds * 1000f);
        return new Color(GetAbsoluteColorChunk(difference.r, duration.TotalMilliseconds),
                        GetAbsoluteColorChunk(difference.g, duration.TotalMilliseconds),
                        GetAbsoluteColorChunk(difference.b, duration.TotalMilliseconds),
                        GetAbsoluteColorChunk(difference.a, duration.TotalMilliseconds));
    }

    public float GetAbsoluteColorChunk(float value, double duration)
    {
        return (float)(Math.Abs(value / duration));
    }
}