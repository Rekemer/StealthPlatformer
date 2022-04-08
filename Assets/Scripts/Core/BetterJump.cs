using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour
{
    //when character falls down
    public float fallMultiplier = 2.5f;
    
    public float lowJumpMultiplier = 2f;

    private Rigidbody2D _rb;

    private float _gravity;
    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // since gravity is calculated in move speed, we must apply it here too
        _gravity = Physics2D.gravity.y * _rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        // make fall a lot quicker
        if (_rb.velocity.y < 0)
        {
            _rb.velocity +=  Vector2.up * _gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (_rb.velocity.y > 0 && !Input.GetButton("Jump")) // to jump lower when jump button is released
        {
            _rb.velocity += Vector2.up * _gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
