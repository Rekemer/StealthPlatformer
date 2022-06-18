using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour
{
    private Animator _animator;
    private PlayerMove _playerMove;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMove = GetComponent<PlayerMove>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        Rigidbody2D playerRb = _playerMove.Rb;
        bool isPlayerGrounded = _playerMove.IsGrounded;
        _animator.SetFloat("xVelocity",playerRb.velocity.x);
        _animator.SetFloat("yVelocity",playerRb.velocity.y);
        _animator.SetBool("IsGrounded",isPlayerGrounded);
    }
}
