using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BetterJump))]
public class Move : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private Detector groundCheck;
    [SerializeField] private Detector leftWallCheck;
    [SerializeField] private Detector rightWallCheck;
    
    
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float horizontalDamping;
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private float timeToJumpApex = .4f;
    [SerializeField] private float wallSlidingSpeed = .4f;

    private float timeToRemember = .2f; // time interval when jump button is pressed - to be able to jump before being grounded

    private Rigidbody2D rb;
    private float time = 0f;
    private bool isGrounded;
    private float xVelocity;
    private bool CanSecondJump;
    private bool spaceBar;
    private float horizontal;
    private bool isWallSliding;
    private bool isWallJumping;
    private bool isTouchingRight;
    private bool isTouchingLeft;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Start()
    {
        var gravity = -2f * jumpHeight / Mathf.Pow(timeToJumpApex, 2);
        var scale = gravity / Physics2D.gravity.y;
        rb.gravityScale = scale;
        jumpVelocity = Mathf.Abs(timeToJumpApex * Physics2D.gravity.y * rb.gravityScale);
    }
    
  
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        spaceBar = Input.GetButtonDown("Jump");
        CheckGround();
        if (isWallSliding)
        {
            CanSecondJump = false;
        }
        else
        {
            if (isGrounded)
            CanSecondJump = true;
        }
        // basic xVelocity 
        xVelocity = horizontal * speed;
        if (isGrounded)
        {
            xVelocity *= Mathf.Pow(1f - horizontalDamping, Time.deltaTime * 10);
        }

        time -= Time.deltaTime;
        // remembering time instead of just checking groung, so we can jump a bit earlier 
        if (spaceBar)
        {
            time = timeToRemember;
        }
        if (isWallSliding && time < 0)
        {
            xVelocity = 0;
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.transform.position, groundCheck.Size, 0f, layerMask);
        isTouchingRight = Physics2D.OverlapBox(rightWallCheck.transform.position, rightWallCheck.Size, 0f, layerMask);
        isTouchingLeft = Physics2D.OverlapBox(leftWallCheck.transform.position, leftWallCheck.Size, 0f, layerMask);
        isWallSliding = (isTouchingLeft || isTouchingRight) && !isGrounded && horizontal != 0;
    }

    private void FixedUpdate()
    {
       
        rb.velocity = new Vector2(xVelocity, rb.velocity.y);
        if (time > 0) // space bar
        {
            DoubleJump();
            WallJumping();
        }
        WallSlide();
    }

    private void WallJumping()
    {
        if (isWallSliding)
        {
            time = 0;
            if (isTouchingRight && xVelocity < 0 || isTouchingLeft && xVelocity > 0)
            {
                rb.velocity = new Vector2(xVelocity, jumpVelocity);
               
            }
        }
    }

    private void WallSlide()
    {
        if (isWallSliding && time <= 0)
        {
            var newYVel = Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue);
            rb.velocity = new Vector2(rb.velocity.x, newYVel);
        }
    }

    private void DoubleJump()
    {
        if (isGrounded)
        {
            Jump();
            CanSecondJump = true;
        }
        else if (CanSecondJump)
        {
            Jump();
            CanSecondJump = false;
        }
    }

    private void Jump()
    {
        // resetting time
        time = 0;
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
    }

    
    
#if UNITY_EDITOR
    // method drawing our ground checkers in scene
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;
        Handles.DrawWireCube(groundCheck.transform.position, groundCheck.Size);
        Handles.DrawWireCube(leftWallCheck.transform.position, leftWallCheck.Size);
        Handles.DrawWireCube(rightWallCheck.transform.position, rightWallCheck.Size);
    }
#endif
}