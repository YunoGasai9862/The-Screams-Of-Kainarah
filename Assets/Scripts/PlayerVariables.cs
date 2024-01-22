
using UnityEngine;

public class PlayerVariables : MonoBehaviour
{
    private static PlayerVariables instance;

    private bool _isJumping;
    private bool _isAttacking;
    private bool _isSliding;
    private bool _isRunning;
    private bool _isWalking;
    private bool _isGrabbing;
    public bool IS_JUMPING { get => _isJumping;  }
    public bool IS_ATTACKING { get => _isAttacking;  }
    public bool IS_SLIDING { get => _isSliding;  }
    public bool IS_RUNNING { get => _isRunning; }
    public bool IS_WALKING { get => _isWalking;  }
    public bool IS_GRABBING { get => _isGrabbing;  }
    public static PlayerVariables Instance { get { return instance; } }

    public PlayerWalkVariableEvent walkVariableEvent = new PlayerWalkVariableEvent();
    public PlayerRunVariableEvent runVariableEvent = new PlayerRunVariableEvent();
    public PlayerSlideVariableEvent slideVariableEvent = new PlayerSlideVariableEvent();
    public PlayerGrabVariableEvent grabVariableEvent = new PlayerGrabVariableEvent();
    public PlayerAttackVariableEvent attackVariableEvent = new PlayerAttackVariableEvent();
    public PlayerJumpVariableEvent jumpVariableEvent = new PlayerJumpVariableEvent();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
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
