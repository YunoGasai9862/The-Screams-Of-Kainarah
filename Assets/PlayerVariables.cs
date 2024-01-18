using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerVariables : MonoBehaviour
{
    private static bool _isJumping;
    private static bool _isAttacking;
    private static bool _isSliding;
    private static bool _isRunning;
    private static bool _isWalking;
    private static bool _isGrabbing;
    public static bool IS_JUMPING { get => _isJumping;  }
    public static bool IS_ATTACKING { get => _isAttacking;  }
    public static bool IS_SLIDING { get => _isSliding;  }
    public static bool IS_RUNNING { get => _isRunning; }
    public static bool IS_WALKING { get => _isWalking;  }
    public static bool IS_GRABBING { get => _isGrabbing;  }

    public static PlayerWalkVariableEvent walkVariableEvent = new PlayerWalkVariableEvent();
    public static PlayerRunVariableEvent runVariableEvent = new PlayerRunVariableEvent();
    public static PlayerSlideVariableEvent slideVariableEvent = new PlayerSlideVariableEvent();
    public static PlayerGrabVariableEvent grabVariableEvent = new PlayerGrabVariableEvent();
    public static PlayerAttackVariableEvent attackVariableEvent = new PlayerAttackVariableEvent();
    public static PlayerJumpVariableEvent jumpVariableEvent = new PlayerJumpVariableEvent();

    private void Start()
    {
        walkVariableEvent.AddListener(SetWalkVariableState);
        runVariableEvent.AddListener(SetRunVariableState);
        slideVariableEvent.AddListener(SetSlideVariableState);
        grabVariableEvent.AddListener(SetGrabVariableState);
        attackVariableEvent.AddListener(SetAttackVariableState);
        jumpVariableEvent.AddListener(SetJumpVariableState);

    }
    private void SetAttackVariableState(bool variableState)
    {
        _isAttacking = variableState;
    }
    private void SetJumpVariableState(bool variableState)
    {
        _isJumping = variableState;
    }
    private void SetSlideVariableState(bool variableState)
    {
        _isSliding = variableState;
    }
    private void SetWalkVariableState(bool variableState)
    {
        _isWalking = variableState;
    }
    private void SetGrabVariableState(bool variableState)
    {
        _isGrabbing = variableState;
    }
    private void SetRunVariableState(bool variableState)
    {
        _isRunning = variableState;
    }
}
