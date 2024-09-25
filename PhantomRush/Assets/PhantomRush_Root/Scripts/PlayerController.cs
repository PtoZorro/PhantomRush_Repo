using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Private References")]
    PlayerInput playerInput;
    Animator anim;

    [Header("Public References")]

    [Header("Input")]

    [Header("Player Stats")]

    [Header("Conditional values")]
    bool isUp, isDown;

    //[Header("Combat Parameters")]

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveUp()
    {
        anim.SetBool("Up", true);
    }
    
    void MoveDown()
    {
        anim.SetBool("Down", true);
    }

    void MoveCenter()
    {

    }

    public void InputMoveUp(InputAction.CallbackContext context)
    {
        if (context.started && !isDown)
        {
            isUp = true;
            MoveUp();
        }
        else if (context.started && isDown)
        {
            MoveCenter();
        }
        else if (context.canceled)
        {
            isUp = false;
            anim.SetBool("Up", false);
        }
    }

    public void InputMoveDown(InputAction.CallbackContext context)
    {
        if (context.started && !isUp)
        {
            isDown = true;
            MoveDown();
        }
        else if (context.canceled)
        {
            isDown = false;
            anim.SetBool("Down", false);
        }
    }
}
