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


    [SerializeField] float _characterSpeed = 10f;

  

    //jumping
    private double initialJumpingSpeed;
    [SerializeField] double maxJumpHeight;
    [SerializeField] double maxJumpTime;



    private double _gravity = -9.81f;
    private float groundedgravity = -.05f;

    private Vector2 _temp = Vector2.zero;

    //gravity -2H/t2
    //v0= -g * peakHeight or 2h/time for peak height

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rocky2DActions = new Rocky2DActions();// initializes the script of Rockey2Dactions
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animationHandler = GetComponent<AnimationHandler>();

        //have the actionMappings
        _rb = GetComponent<Rigidbody2D>();

        _rocky2DActions.PlayerMovement.Jump.performed += Jump;
        _rocky2DActions.PlayerMovement.Jump.canceled += JumpCancel;

        initializingJumpVariables(); 
    }

    private void initializingJumpVariables()
    {
        double timeforApex = maxJumpTime / 2;
        _gravity = ((-2 * maxJumpHeight) / Math.Pow(timeforApex, 2)); //till half
        initialJumpingSpeed = (2 * maxJumpHeight) / timeforApex; 

    }

    private void Start()
    {
        _rocky2DActions.PlayerMovement.Enable(); //enables that actionMap =>Movement

    }
  

    private void FixedUpdate()
    {
        //Movement

        _keystrokeTrack = PlayerMovement();

        //_= (_jumpValue==1) ? _rb.velocity = new Vector2(_rb.velocity.x, JumpSpeed) : _rb.velocity = new Vector2(_rb.velocity.x, -JumpSpeed/100);

     
      

    }

    private void Update()
    {
        FlipCharacter(_keystrokeTrack, ref _spriteRenderer);

        if (_isJumping)
        {
            _temp.y = (float)initialJumpingSpeed;
            _rb.velocity = _temp;
                
        }
       
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
