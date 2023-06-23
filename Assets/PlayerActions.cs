using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    private PlayerInput _playerInput;
    private Rocky2DActions _rocky2DActions;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private AnimationHandler _animationHandler;
    private Vector2 _keystrokeTrack;
    private bool _isJumping;
    private IOverlapChecker _movementHelperClass;
    private BoxCollider2D _boxCollider;

    [SerializeField] float _characterSpeed = 10f;
    [SerializeField] LayerMask jumpLayer;
    [SerializeField] double maxJumpHeight;


    private double _gravityScale = 6;
    private double _jumpForce;

    //Force = -2m * sqrt (g * h)

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rocky2DActions = new Rocky2DActions();// initializes the script of Rockey2Dactions
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animationHandler = GetComponent<AnimationHandler>();
        _movementHelperClass= new MovementHelperClass();
        _boxCollider = GetComponent<BoxCollider2D>();

        //have the actionMappings
        _rb = GetComponent<Rigidbody2D>();

        _rocky2DActions.PlayerMovement.Jump.performed += Jump;
        _rocky2DActions.PlayerMovement.Jump.canceled += JumpCancel;


    }

    private void Start()
    {
        _rocky2DActions.PlayerMovement.Enable(); //enables that actionMap =>Movement

    }
  

    private void FixedUpdate()
    {
        //Movement

        _keystrokeTrack = PlayerMovement();

        //Jumping
        if (_isJumping && _movementHelperClass.overlapAgainstLayerMaskChecker(ref _boxCollider, jumpLayer))
        {
            HandleJumping();
           
        }

        if(!_isJumping && !_movementHelperClass.overlapAgainstLayerMaskChecker(ref _boxCollider, jumpLayer))
        {
            _rb.gravityScale += (float)_gravityScale;

        }

        if(_movementHelperClass.overlapAgainstLayerMaskChecker(ref _boxCollider, jumpLayer))
        {
            _rb.gravityScale = 1f;
        }

    }

    public void HandleJumping()
    {
        _rb.gravityScale -=(float)_gravityScale;
        _jumpForce = Math.Sqrt(maxJumpHeight * (Physics2D.gravity.y * Math.Abs(_gravityScale) * -2)) * _rb.mass;
        _rb.velocity += new Vector2(_rb.velocity.x, (float)_jumpForce);
        _isJumping = false;


    }


    private void Update()
    {
        FlipCharacter(_keystrokeTrack, ref _spriteRenderer);

    }
    private Vector2 PlayerMovement()
    {
         Vector2 keystroke = _rocky2DActions.PlayerMovement.Movement.ReadValue<Vector2>(); //reads the value

        _rb.velocity = new Vector2(keystroke.x, 0) * _characterSpeed;

        _animationHandler.RunningWalkingAnimation(keystroke.x);  //for movement, plays the animation

        return keystroke;
    }

    private bool FlipCharacter(Vector2 keystroke, ref SpriteRenderer _sr)
    {
        return keystroke.x >= 0 ? _sr.flipX=false : _sr.flipX=true; //flips the character

    }

    private void Jump(InputAction.CallbackContext context)
    {

        if(context.performed)
        {
            _isJumping = context.ReadValueAsButton();

            _animationHandler.JumpingFalling(_isJumping);

        }
    }

    private void JumpCancel(InputAction.CallbackContext context)
    {

        if (context.canceled)
        {
            _isJumping = context.ReadValueAsButton();

            _animationHandler.JumpingFalling(_isJumping);

        }
    }




}
