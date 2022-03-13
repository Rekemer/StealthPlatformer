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

    private float timeToRemember = .2f; // time interval when jump button is pressed - to be able to jump before being grounded

    private Rigidbody2D rb;
    private float time =0f;
    private bool isGrounded;
    private float xVelocity;
    private bool spaceBarBefore;
    private bool spaceBar;
    private float horizontal;
    
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
        Debug.Log("calculated gravity" + gravity);
        Debug.Log("real gravity" +  Physics2D.gravity.y * rb.gravityScale);
        Debug.Log("jumpVelocity" + jumpVelocity);
    }


    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.transform.position, groundCheck.Size, angle, layerMask);
        horizontal = Input.GetAxis("Horizontal");
        spaceBar = Input.GetButtonDown("Jump");
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
        rb.velocity = new Vector2(xVelocity, rb.velocity.y);
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