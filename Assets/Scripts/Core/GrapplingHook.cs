using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.EventSystems;

public class GrapplingHook : MonoBehaviour
{
    // in what way we getting closer to hook 
    [SerializeField] private LerpClass.InterType _interType;
    [SerializeField] private float _speed;
    [SerializeField] private float _ropeMaxCastDistance;
    [SerializeField] private RopeAnchor _ropeAnchor;
    [SerializeField] private float _coolDown;
    
    private bool _isHookAvailable = true;
    private bool _isRopeAttached;
    private Vector2 _hitPos;
    
    public SpriteRenderer crosshairSprite;
    public Transform crosshair;
    public LineRenderer ropeRenderer;
    public LayerMask ropeLayerMask;
    public float CooldownTime => _coolDown;

    private bool _gotBonus;
    public bool GotBonus
    {
        get
        {
            return _gotBonus;
        }
        set
        {
            _gotBonus = true;
            if (_isHookAvailable == false)
            {
                // just apply bonus
                EventSystem.current.ActivateBonus();
                ResetHook(true);
                _gotBonus = false;
            }
            else
            {
                // unhook once bonus is got
               Unhook();
               EventSystem.current.ActivateBonus();
               ResetHook(true);
               
               // have hook to spare once bonus is got
               //StartCoroutine(WaitingForEndOfHooking());
            }
        }
        
    }

    IEnumerator WaitingForEndOfHooking()
    {
        while (!_isHookAvailable)
        {
                ResetHook(true);
                EventSystem.current.ActivateBonus();
                _gotBonus = false;
                yield return null;
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        ropeRenderer.enabled = false;
        _ropeAnchor.gameObject.SetActive(false);
        crosshair.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, Camera.main.nearClipPlane));
        var facingDirection = worldMousePosition - transform.position;
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);

        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        SetCrosshairPosition(worldMousePosition);
        var aimDirection = new Vector2(Mathf.Cos(aimAngle), Mathf.Sin(aimAngle));
        HandleInput(aimDirection);
        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, _hitPos);
        _ropeAnchor.transform.position = _hitPos;
    }

    private void SetCrosshairPosition(Vector3 mousePos)
    {
        if (!crosshairSprite.enabled)
        {
            crosshairSprite.enabled = true;
        }

        crosshair.transform.position = mousePos;
    }


    private void SetCrosshairPosition(float aimAngle)
    {
        if (!crosshairSprite.enabled)
        {
            crosshairSprite.enabled = true;
        }

        var x = transform.position.x + Mathf.Cos(aimAngle);
        var y = transform.position.y + Mathf.Sin(aimAngle);

        var crossHairPosition = new Vector3(x, y, 0);
        crosshair.transform.position = crossHairPosition;
    }

    private void HandleInput(Vector2 aimDirection)
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 2
            if (_isRopeAttached || !_isHookAvailable) return;

            ropeRenderer.enabled = true;

            var hit = Physics2D.Raycast(transform.position, aimDirection, _ropeMaxCastDistance, ropeLayerMask);

            // 3
            if (hit.collider != null)
            {
                _hitPos = hit.point;
                ropeRenderer.SetPosition(0, transform.position);
                ropeRenderer.SetPosition(1, _hitPos);
                _isRopeAttached = true;
               
                // start hooking
                Hook();
            }
            // 5
            else
            {
                ropeRenderer.enabled = false;
                _isRopeAttached = false;
            }
        }


        if (Input.GetMouseButtonDown(1))
        {
            // check if hooking
            if (_isRopeAttached)
            {
                Unhook();
                _isRopeAttached = false;
            }

            ResetRope();
        }
    }

    public IEnumerator StartCooldown()
    {
        if (_gotBonus == false)
        ResetHook(false);
        // start to fill timer (call method)
        yield return CooldownRoutine(_coolDown);
        ResetHook(true);
    }

    IEnumerator CooldownRoutine(float coolDown)
    {
        float time = 0f;
        while (time <=coolDown && !_isHookAvailable)
        {
            time += Time.deltaTime;
            Debug.Log(time);
            yield return null;
        }
    }
    private void ResetHook(bool state)
    { 
        // reset timer
        _isHookAvailable = state;
    }
    
    private void Hook()
    {
        _ropeAnchor.gameObject.SetActive(true);
        StartCoroutine(HookRoutine());
    }

    IEnumerator HookRoutine()
    {
        float t = 0f;
        GetComponent<PlayerMove>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        float curSpeed = 0f;
        while (_isRopeAttached)
        {
            float acceleration = LerpClass.Lerp(t, _interType);
            var direction = ((Vector3) _hitPos - transform.position).normalized;
            curSpeed = Mathf.Clamp(acceleration * _speed, 0f, _speed);
            var vel = (Vector2) direction * curSpeed;
            GetComponent<Rigidbody2D>().velocity = vel;
            t += Time.deltaTime;
            yield return null;
        }
    }

    private void Unhook()
    {
        _ropeAnchor.gameObject.SetActive(false);
        _hitPos = transform.position;
        GetComponent<PlayerMove>().enabled = true;
        ResetRope();
        StartCoroutine(StartCooldown());
        EventSystem.current.OnGrapplingHookDeactivation(_coolDown);

    }

    private void ResetRope()
    {
        _isRopeAttached = false;
        ropeRenderer.positionCount = 2;
        ropeRenderer.enabled = false;
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, transform.position);
    }

    void OnTriggerEnter2D(Collider2D colliderStay)
    {
        if (colliderStay.gameObject.CompareTag("Anchor"))
        {
            Unhook();
        }
    }

    private void OnTriggerExit2D(Collider2D colliderOnExit)
    {
        if (colliderOnExit.gameObject.CompareTag("Anchor"))
        {
            
        }
    }
}