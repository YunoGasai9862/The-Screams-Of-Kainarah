using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    private PlayerInput _playerInput;
    private Rocky2DActions _rocky2DActions;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _keystrokeTrack;
    [SerializeField] float _characterSpeed = 10f;
    [SerializeField] float JumpSpeed = 10f;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rocky2DActions = new Rocky2DActions();// initializes the script of Rockey2Dactions
        _spriteRenderer = GetComponent<SpriteRenderer>();
        //have the actionMappings
        _rb= GetComponent<Rigidbody2D>();

        _rocky2DActions.PlayerMovement.Jump.performed += Jump;
    }

  

    private void Start()
    {
        _rocky2DActions.PlayerMovement.Enable(); //enables that actionMap

    }

    private void FixedUpdate()
    {
        //Movement

        _keystrokeTrack = PlayerMovement();

        
    }

    private void Update()
    {
        FlipCharacter(_keystrokeTrack, ref _spriteRenderer);

    }
    private Vector2 PlayerMovement()
    {
         Vector2 keystroke = _rocky2DActions.PlayerMovement.Movement.ReadValue<Vector2>(); //reads the value

        _rb.velocity = new Vector2(keystroke.x, 0) * _characterSpeed;

        return keystroke;
    }

    private bool FlipCharacter(Vector2 keystroke, ref SpriteRenderer _sr)
    {
        return keystroke.x >= 0 ? _sr.flipX=false : _sr.flipX=true; //flips the character

    }

    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        if(context.performed)
        {
            _rb.AddForce(Vector3.up * JumpSpeed, ForceMode2D.Impulse);
        }
    }




}
