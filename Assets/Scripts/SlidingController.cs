using CoreCode;
using GlobalAccessAndGameHelper;
using PlayerAnimationHandler;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SlidingController : MonoBehaviour, IReceiver<bool>
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] float slidingSpeed;

    private PlayerAnimationMethods _animationHandler;
    private IOverlapChecker _movementHelperClass;
    private PlayerAttackStateMachine _playerAttackStateMachine;
    private CapsuleCollider2D _capsuleCollider;
    private Animator _anim;
    private float _characterVelocityX;
    public float CharacterVelocityX { get => _characterVelocityX; set => _characterVelocityX = value; }

    public OnSlidingEvent onSlideEvent;
    void Start()
    {
        _animationHandler = GetComponent<PlayerAnimationMethods>();
        _movementHelperClass = GetComponent<IOverlapChecker>();
        _anim = GetComponent<Animator>();
        _playerAttackStateMachine = new PlayerAttackStateMachine(_anim);
        _capsuleCollider= GetComponent<CapsuleCollider2D>();    
        onSlideEvent = new OnSlidingEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CancelAction()
    {
        return false;
    }

    public bool PerformAction(bool value)
    {
        PlayerMovementGlobalVariables.ISSLIDING = value;

        _animationHandler.Sliding(PlayerMovementGlobalVariables.ISSLIDING); //set animation

        PlayerMovementHelperFunctions.setAttacking(false); //for some minor fixes


        if (PlayerMovementHelperFunctions.boolConditionAndTester(PlayerMovementGlobalVariables.ISSLIDING, !PlayerMovementGlobalVariables.ISATTACKING,
      _movementHelperClass.overlapAgainstLayerMaskChecker(ref _capsuleCollider, groundLayer),
      !_playerAttackStateMachine.isInEitherOfTheAttackingStates<PlayerAttackEnum.PlayerAttackSlash>()))
        {
            _playerAttackStateMachine.ForceDisableAttacking(1);

            onSlideEvent.Invoke(CharacterVelocityX * slidingSpeed); //posting

            //CharacterControllerMove(CharacterVelocityX * slidingSpeed, CharacterVelocityY);

        }

        if (_animationHandler.returnCurrentAnimation() > .6f && _animationHandler.isNameOfTheCurrentAnimation(AnimationConstants.SLIDING))
        {
            PlayerMovementHelperFunctions.setSliding(false);
        }

        //implement this condition here instead
        if (PlayerMovementHelperFunctions.boolConditionAndTester(!PlayerMovementGlobalVariables.ISJUMPING,
         !_playerAttackStateMachine.isInEitherOfTheAttackingStates<PlayerAttackEnum.PlayerAttackSlash>())) //conditions for sliding
        {


        }
        return true;
    }

    private Task Slide()
    {
        return Task.CompletedTask;
    }
}
