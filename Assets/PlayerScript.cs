using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerScript : MonoBehaviour
{

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private Rocky2DGamePlayerInput rocky2dgameplayerinput; //member fields

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();


        rocky2dgameplayerinput = new Rocky2DGamePlayerInput(); //creative the script object
        rocky2dgameplayerinput.PlayerInput.Enable();
      
        rocky2dgameplayerinput.PlayerInput.Jump.performed += Jump;

      // rocky2dgameplayerinput.PlayerInput.Movement.performed += MovingtheWizard;
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

       // playerInput.onActionTriggered += PlayerInput_onActionTriggeredWizard;
        //fires an event
    }

    private void Update()
    {
        if(Keyboard.current.tKey.wasPressedThisFrame)  //for debugging!
        {

            playerInput.SwitchCurrentActionMap("UI");//when t is presssed, it switches the action mapping!
            rocky2dgameplayerinput.PlayerInput.Disable();
            Debug.Log("UI");
            //clicked
        }

        if (Keyboard.current.yKey.wasPressedThisFrame)  //for debugging!
        {

            playerInput.SwitchCurrentActionMap("PlayerInput");//when y is presssed, it switches the action mapping!
            rocky2dgameplayerinput.UI.Disable();


            Debug.Log("PlayerInput");
            //clicked
        }
    }
    private void FixedUpdate()
    {
        Vector2 input=  rocky2dgameplayerinput.PlayerInput.Movement.ReadValue<Vector2>();
       // Debug.Log(input);
        float speed = 10f;
        
        rb.AddForce(new Vector2(input.x, input.y) * speed, ForceMode2D.Force);
    }



    /**
    private void PlayerInput_onActionTriggeredWizard(InputAction.CallbackContext context)
    {
        Debug.Log(context); //all the actions will fire
    }
    **/

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            rb.AddForce(Vector3.up * 5f, ForceMode2D.Impulse);
            Debug.Log("Jumping..." + context.phase);
        }
      
    }

    public void Submit(InputAction.CallbackContext context)
    {
       Debug.Log("Submit" + context);

    }

}
