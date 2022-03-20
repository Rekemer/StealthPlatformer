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
    [SerializeField] private Vector2 radius;
    [SerializeField] private float angle;
    [SerializeField] private float horizontalDamping;
    [SerializeField] private bool check;
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private float timeToJumpApex = .4f;
    [SerializeField] private float wallSlidingSpeed = .4f;
    [SerializeField] private float xWallJump = 3f;
    [SerializeField] private float yWallJump = 3f;

    private float timeToRemember = .2f; // time interval when jump button is pressed - to be able to jump before being grounded

    private Rigidbody2D rb;
    private float time =0f;
    private bool isGrounded;
    private float xVelocity;
    private bool spaceBarBefore;
    private bool spaceBar;
    private float horizontal;
    private bool isWallSliding;
    private bool isWallJumping;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var gravity = -2f * jumpHeight / Mathf.Pow(timeToJumpApex, 2);
        var scale = gravity / Physics2D.gravity.y;
        rb.gravityScale = scale;
        jumpVelocity = Mathf.Abs(timeToJumpApex * Physics2D.gravity.y * rb.gravityScale);
        // Debug.Log("calculated gravity" + gravity);
        // Debug.Log("real gravity" +  Physics2D.gravity.y * rb.gravityScale);
        // Debug.Log("jumpVelocity" + jumpVelocity);
    }


    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        spaceBar = Input.GetButtonDown("Jump");
        
        isGrounded = Physics2D.OverlapBox(groundCheck.transform.position, groundCheck.Size, angle, layerMask);
        bool isTouchingRight = Physics2D.OverlapBox(rightWallCheck.transform.position, rightWallCheck.Size,0f, layerMask);
        bool isTouchingLeft = Physics2D.OverlapBox(leftWallCheck.transform.position, leftWallCheck.Size, 0f,layerMask);
        isWallSliding = (isTouchingLeft || isTouchingRight ) && !isGrounded && horizontal!=0;
        if (isWallSliding && spaceBar)
        {
            isWallJumping = true;
            Invoke("ResetWallJump",0.2f ); 
        }
        
        Debug.Log("isWallJumping " + isWallJumping);
        if (isTouchingRight)
        {
            Debug.Log("touching right" + isTouchingRight);
        }
        
        if (isTouchingLeft)
        {
            Debug.Log("touching left" + isTouchingLeft);
        }
        
       
        xVelocity = horizontal * speed;
        if (check)
        {
            xVelocity *= Mathf.Pow(1f - horizontalDamping, Time.deltaTime * 10);
        }

        time -= Time.deltaTime;
        if (spaceBar)
        {
            time = timeToRemember;
        }
    }

    private void FixedUpdate()
    {
        if (time > 0)
        {
            if (isGrounded)
            {
                time = 0;
                rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
                spaceBarBefore = true;
            }
            else if (spaceBarBefore)
            {
                time = 0;
                rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
                spaceBarBefore = false;
            }
        }

        if (isWallSliding)
        {
            if (isWallJumping)
            {
                var newVelocity = new Vector2(xWallJump * -horizontal, yWallJump);
                rb.velocity = newVelocity;
               
            }
            else
            {
                var newYVel = Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue);
                rb.velocity = new Vector2(rb.velocity.x, newYVel) ;
            }
            
        }
        if (!isWallJumping)
        rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    }

    private void ResetWallJump()
    {
        isWallJumping = false;
      
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //groundCheck.position = 
        Handles.color = Color.red;
        Handles.DrawWireCube(groundCheck.transform.position, groundCheck.Size);
        Handles.DrawWireCube(leftWallCheck.transform.position, leftWallCheck.Size);
        Handles.DrawWireCube(rightWallCheck.transform.position, rightWallCheck.Size);
    }
#endif
}