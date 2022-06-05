using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BetterJump))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _speed;

    [SerializeField] private Detector _groundCheck;
    [SerializeField] private Detector _leftWallCheck;
    [SerializeField] private Detector _rightWallCheck;


    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _horizontalDamping;
    [SerializeField] private float _jumpHeight = 4f;
    [SerializeField] private float _timeToJumpApex = .4f;
    [SerializeField] private float _wallSlidingSpeed = .4f;
    [SerializeField] private float _timeWallStick;
    private float _timeWallUnstick;
    private float _jumpVelocity;

    private float
        _timeToRemember = .2f; // time interval when jump button is pressed - to be able to jump before being grounded

    private Rigidbody2D _rb;
    private float _time = 0f;
    private bool _isGrounded;
    private float _xVelocity;
    private bool _CanSecondJump;
    private bool _spaceBar;
    private float _horizontal;
    private bool _isWallSliding;
    private bool _isWallJumping;
    private bool _isTouchingRight;
    private bool _isTouchingLeft;
    private bool _isRightButtonPressed;
    private bool _hasBeenInvoked;
    private bool _leftPrevious;
    private bool _rightPrevious;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }


    // using physics equation to calculate parabola trajectory jumping
    //https://www.youtube.com/watch?v=hG9SzQxaCm8&t=517s&ab_channel=GDC
    void Start()
    {
        var gravity = -2f * _jumpHeight / Mathf.Pow(_timeToJumpApex, 2);
        var scale = gravity / Physics2D.gravity.y;
        _rb.gravityScale = scale;
        _jumpVelocity = Mathf.Abs(_timeToJumpApex * Physics2D.gravity.y * _rb.gravityScale);
    }

    
    void Update()
    {
        _horizontal = Input.GetAxis("Horizontal");
        
        _spaceBar = Input.GetButtonDown("Jump");
        _isRightButtonPressed = Input.GetMouseButton(1);
        CheckCollisions();
        if (_isWallSliding)
        {
            _CanSecondJump = false;
        }
        else
        {
            if (_isGrounded)
                _CanSecondJump = true;
        }

        // basic xVelocity 
        _xVelocity = _horizontal * _speed;
        if (_isGrounded)
        {
            _xVelocity *= Mathf.Pow(1f - _horizontalDamping, Time.deltaTime * 10);
        }

        _time -= Time.deltaTime;
        // remembering time instead of just checking groung, so we can jump a bit earlier 
        if (_spaceBar)
        {
            _time = _timeToRemember;
        }
    }


    private void CheckCollisions()
    {
        _isGrounded = Physics2D.OverlapBox(_groundCheck.transform.position, _groundCheck.Size, 0f, _layerMask);
        _isTouchingLeft = CastRayToDirection(Vector2.left);
         _isTouchingRight = CastRayToDirection(Vector2.right);
        UpdateWallSliding();
    }

    private void UpdateWallSliding()
    {
        // if (_isWallSliding)
        // {
        //     
        //
        //     // if we wall slide and want to move from wall - give time interval for wall jumping
        //     if (!_hasBeenInvoked && (_isTouchingLeft && _horizontal > 0 || _isTouchingRight && _horizontal < 0))
        //     {
        //         Invoke("ResetWallSliding", 0.3f);
        //         _hasBeenInvoked = true;
        //     }
        // }

        var hitLeft = CastRayToDirection(Vector2.left);
        var hitRight = CastRayToDirection(Vector2.right);
        _isWallSliding = ((hitLeft || hitRight) &&  _isRightButtonPressed) && !_isGrounded;
    }

   

    void ResetWallSliding()
    {
        _isWallSliding = false;
        _hasBeenInvoked = false;
    }

    private void FixedUpdate()
    {
        ApplyVelocity();

        if (_time > 0) // space bar
        {
            DoubleJump();
            WallJumping();
        }

        WallSlide();
    }

    bool CastRayToDirection(Vector2 direction)
    {
        direction.y = 0;
        direction.Normalize();
        var hit = Physics2D.Raycast(transform.position, direction, 0.7f, _layerMask);
        return  hit;
    }
    private void ApplyVelocity()
    {
        if (_isWallSliding)
        {
            _rb.velocity = new Vector2(_xVelocity, _rb.velocity.y);
            return;
        }
        
        
        Vector3 horizontalMove = new Vector2(_xVelocity, _rb.velocity.y);
        horizontalMove.y = 0;
        horizontalMove.Normalize();
        var hit = Physics2D.Raycast(transform.position+ (Vector3)Vector2.up * 0.5f, horizontalMove, 0.55f, _layerMask);
        var hit1 = Physics2D.Raycast(transform.position+ (Vector3)Vector2.down * 0.5f, horizontalMove, 0.55f, _layerMask);
        if (hit || hit1)
        {
           
            // If so, stop the movement
            _rb.velocity = new Vector3(0, _rb.velocity.y);
        }
        else
        {
            _rb.velocity = new Vector2(_xVelocity, _rb.velocity.y);
        }
    }

    private void WallJumping()
    {
        if (_isWallSliding)
        {
            _time = 0;
            if (_isTouchingRight && _horizontal < 0 || _isTouchingLeft && _horizontal > 0)
            {
                
                Debug.Log("jmp");
                _rb.velocity = new Vector2(_xVelocity, _jumpVelocity);
                // _CanSecondJump = true;
            }
        }
    }

   
    

    private void WallSlide()
    {
      
        if (_isWallSliding )
        {
            
            var newYVel = Mathf.Clamp(_rb.velocity.y, -_wallSlidingSpeed, float.MaxValue);
            _rb.velocity = new Vector2(_rb.velocity.x, newYVel);
        }
    }

    private void DoubleJump()
    {
        if (_isGrounded)
        {
            Jump();
            _CanSecondJump = true;
        }
        else if (_CanSecondJump)
        {
            //_rb.AddForce(Vector2.up * _jumpVelocity,ForceMode2D.Impulse);
            Jump();
            _CanSecondJump = false;
        }
    }

    private void Jump()
    {
        // resetting time
        _time = 0;
        _rb.velocity = new Vector2(_rb.velocity.x, _jumpVelocity);
    }


#if UNITY_EDITOR
    // method drawing our ground checkers in scene
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;
        Handles.DrawWireCube(_groundCheck.transform.position, _groundCheck.Size);
        Handles.DrawWireCube(_leftWallCheck.transform.position, _leftWallCheck.Size);
        Handles.DrawWireCube(_rightWallCheck.transform.position, _rightWallCheck.Size);
    }
#endif
}