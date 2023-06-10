using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    private PlayerInput _playerInput;
    private Rocky2DActions _rocky2DActions;
    private void Start()
    {
        _playerInput= GetComponent<PlayerInput>(); 
        _rocky2DActions= new Rocky2DActions();// initializes the script of Rockey2Dactions
        //have the actionMappings

        
        _rocky2DActions.PlayerMovement.Enable(); //enables that actionMap
        _rocky2DActions.PlayerMovement.Movement.performed += Movement; //added it to perform only

    }

    private void Movement(InputAction.CallbackContext callbackContext)
    {
        //PlayerMovement
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
