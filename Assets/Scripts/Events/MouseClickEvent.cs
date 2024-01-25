
using System;
using UnityEngine.Events;

public class MouseClickEvent : UnityEvent<float, float>
{
    private float _startTime;
    private float _endTime;
    public float ClickStartTime { get => _startTime; set => _startTime = value; }
    public float ClickEndTime { get => _endTime; set => _endTime = value; }
    public MouseClickEvent(float startTime, float endTime)
    {
        _startTime = startTime;
        _endTime = endTime; 
    }
    public MouseClickEvent()
    {
        _startTime = 0f;
        _endTime = 0f;
    }
    public float TimeDifferenceBetweenPressAndRelease()
    {
        return Math.Abs(_endTime - _startTime);
    }
}
