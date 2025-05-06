using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCPlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;

    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;
    bool dash = false;

    //bool dashAxis = false;
    // TODO maybe add a downward dash/ground pound later, that could also be used for moveent tech?

    // Update is called once per frame
    void Update()
    {
        // horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed; // old input sstem
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
        {
            // TODO add hold to stay up longer
            Debug.Log($"SPACE {horizontalMove}");
            jump = true;
        }

        // TODO fix input system
        // ALERT bug known with clipping through solid surfaces on dash
        if (Input.GetButtonDown("Dash") || Input.GetKeyDown(KeyCode.RightShift))
        {
            // TODO add hold to dash farther
            dash = true;
        }

        /*if (Input.GetAxisRaw("Dash") == 1 || Input.GetAxisRaw("Dash") == -1) //RT in Unity 2017 = -1, RT in Unity 2019 = 1
        {
            if (dashAxis == false)
            {
                dashAxis = true;
                dash = true;
            }
        }
        else
        {
            dashAxis = false;
        }
        */
    }

    public void OnFall()
    {
        animator.SetBool("IsJumping", true);
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash);
        jump = false;
        dash = false;
    }
}
