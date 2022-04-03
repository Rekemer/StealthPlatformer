using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour
{
    //when character falls down
    public float fallMultiplier = 2.5f;
    
    public float lowJumpMultiplier = 2f;

    private Rigidbody2D rb;

    private float gravity;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // since gravity is calculated in move speed, we must apply it here too
        gravity = Physics2D.gravity.y * rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        // make fall a lot quicker
        if (rb.velocity.y < 0)
        {
            rb.velocity +=  Vector2.up * gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) // to jump lower when jump button is released
        {
            rb.velocity += Vector2.up * gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
