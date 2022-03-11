using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    private Controller2D controller;
    private float gravity = -5f;
    private Vector2 velocity;
    private void Awake()
    {
        controller = GetComponent<Controller2D>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        velocity.y += Time.deltaTime * gravity;
        controller.Move(velocity);
    }
}
