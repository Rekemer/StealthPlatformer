using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Move : MonoBehaviour
{
    
    [SerializeField] private float speed;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Vector2 radius;
    [SerializeField] private float angle;
    [SerializeField] private float horizontalDamping;
    [SerializeField] private bool check;
    private float timeToRemember = .2f; // time interval when jump button is pressed - to be able to jump before being grounded
    private Rigidbody2D rb;
    private float time;
    private bool isGrounded;
    private float xVelocity;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.position, radius, angle, layerMask);
        var horizontal = Input.GetAxis("Horizontal");
        var spaceBar = Input.GetButtonDown("Jump");
        xVelocity =  horizontal * speed;
        if (check)
        {
            xVelocity *= Mathf.Pow(1f - horizontalDamping, Time.deltaTime * 10);
        }
        Debug.Log(isGrounded);
       
        time -= Time.deltaTime;
        if (spaceBar)
        {
            time = timeToRemember;
        }
        
        if ( time > 0 && isGrounded)
        {
            time = 0;
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }
        rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //groundCheck.position = 
        Handles.color = Color.red;
        Handles.DrawWireCube(groundCheck.position, radius);
    } 
#endif
    
}
