using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerScript : MonoBehaviour
{

    private Rigidbody2D rb;
    // private PlayerInput playerInput;

    private void Awake()
    {
        Rocky2DGamePlayerInput rocky2dgameplayerinput= new Rocky2DGamePlayerInput(); //creative the script object
        rocky2dgameplayerinput.PlayerInput.Enable();
      
        rocky2dgameplayerinput.PlayerInput.Jump.performed += Jump;

        rocky2dgameplayerinput.PlayerInput.Movement.performed += MovingtheWizard;
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
       // playerInput = GetComponent<PlayerInput>();

       // playerInput.onActionTriggered += PlayerInput_onActionTriggeredWizard;
        //fires an event
    }

    public void MovingtheWizard(InputAction.CallbackContext context)
    {

        Vector2 input = context.ReadValue<Vector2>();
        float speed = 5f;

        rb.AddForce(new Vector2(input.x, input.y) * speed, ForceMode2D.Impulse);


    }

    /**
    private void PlayerInput_onActionTriggeredWizard(InputAction.CallbackContext context)
    {
        Debug.Log(context); //all the actions will fire
    }
    **/

    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        if(context.performed)
        {
            rb.AddForce(Vector3.up * 5f, ForceMode2D.Impulse);
            Debug.Log("Jumping..." + context.phase);
        }
      
    } 
}
