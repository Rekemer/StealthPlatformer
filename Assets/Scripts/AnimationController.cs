using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class AnimationController : MonoBehaviour
{
    private Animator _animator;
    private PlayerMove _playerMove;
    private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _playerMove = transform.parent.GetComponent<PlayerMove>();
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
        _spriteRenderer.flipX = Mathf.Sign(_playerMove.Rb.velocity.x) == -1;
        _animator.SetFloat("xVelocity",playerRb.velocity.x);
        _animator.SetFloat("yVelocity",playerRb.velocity.y);
        _animator.SetBool("IsGrounded",isPlayerGrounded);
    }
}
