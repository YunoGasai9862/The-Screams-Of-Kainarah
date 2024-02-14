using UnityEngine;
using UnityEngine.Events;

public class PlayerJumpTimeEvent : UnityEvent<float, float>
{
    private float _jumpActionBeginTime;
    private float _jumpActionEndTime;
    private float _jumpTime;

    private const float MAX_JUMP_TIME = 0.3f;
    public float JumpActionBeginTime { get => _jumpActionBeginTime; set => _jumpActionBeginTime = value; }
    public float JumpActionEndTime { get => _jumpActionEndTime; set => _jumpActionEndTime = value; }
    public PlayerJumpTimeEvent() { }
    public bool IsJumpTimeWithinAcceptableRange()
    {
        return Mathf.Abs(JumpActionBeginTime - JumpActionEndTime) < MAX_JUMP_TIME;
    }
    public float CalculateTime(float beginTime)
    {
        if (beginTime + _jumpTime == MAX_JUMP_TIME)
        {
            _jumpTime = 0;
            return 0;
        }

        _jumpTime += Time.deltaTime;
        return CalculateTime(beginTime);
    }

}