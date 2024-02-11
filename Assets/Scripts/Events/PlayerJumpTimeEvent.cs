using UnityEngine.Events;

public class PlayerJumpTimeEvent : UnityEvent<float, float>
{
    private float _jumpActionBeginTime;
    private float _jumpActionEndTime; 
    
    public float JumpActionBeginTime {  get => _jumpActionBeginTime;  set => _jumpActionBeginTime = value; }
    public float JumpActionEndTime { get => _jumpActionEndTime; set => _jumpActionEndTime = value; }
    PlayerJumpTimeEvent() { }

}