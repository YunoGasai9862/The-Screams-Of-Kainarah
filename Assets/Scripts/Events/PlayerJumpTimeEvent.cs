using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerJumpTimeEvent : UnityEvent<float, float ,bool>
{
    private float _jumpActionBeginTime;
    private float _maxJumpTime;
    private bool _isJumping = true;

    private float _jumpTime = 0f;
    public float JumpActionBeginTime { get => _jumpActionBeginTime; set => _jumpActionBeginTime = value; }
    public bool IsJumping { get => _isJumping; set => _isJumping = value;}
    public float MaxTimeToJump { get => _maxJumpTime; set => _maxJumpTime = value; }
    public PlayerJumpTimeEvent() { }

    public IEnumerator CanPlayerKeepJumping()
    {
        while(JumpActionBeginTime + _jumpTime < JumpActionBeginTime + MaxTimeToJump && IsJumping)
        {
            _jumpTime += Time.deltaTime;
        }

        _jumpTime = 0f;
        IsJumping = false;
        yield return new WaitForSeconds(0f);
    }
} 