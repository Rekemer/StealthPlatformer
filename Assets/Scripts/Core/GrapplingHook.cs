using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    // in what way we getting closer to hook 
    [SerializeField] private LerpClass.InterType interType;
    [SerializeField] private float speed;
    [SerializeField] private float ropeMaxCastDistance;
    [SerializeField] private RopeAnchor ropeAnchor;
    public SpriteRenderer crosshairSprite;
    public Transform crosshair;
    public LineRenderer ropeRenderer;
    public LayerMask ropeLayerMask;
    
    private bool ropeAttached;

    private bool isHooking;
    private Vector2 hitPos;
    // Start is called before the first frame update
    void Start()
    {
        ropeRenderer.enabled = false;
        ropeAnchor.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        var facingDirection = worldMousePosition - transform.position;
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);

        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        SetCrosshairPosition(aimAngle);
        var aimDirection = new Vector2( Mathf.Cos(aimAngle), Mathf.Sin(aimAngle));
        HandleInput(aimDirection);
        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, hitPos);
        ropeAnchor.transform.position = hitPos;
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
        if (Input.GetMouseButton(0))
        {
            // 2
            if (ropeAttached) return;
            ropeRenderer.enabled = true;

            var hit = Physics2D.Raycast(transform.position, aimDirection, ropeMaxCastDistance, ropeLayerMask);

            // 3
            if (hit.collider != null)
            {
                
                
                hitPos = hit.point;
                ropeRenderer.SetPosition(0, transform.position);
                ropeRenderer.SetPosition(1, hitPos);
                ropeAttached = true;
                // start hooking
                 Hook();
                
            }
            // 5
            else
            {
                ropeRenderer.enabled = false;
                ropeAttached = false;
            }
        }

        if (Input.GetMouseButton(1))
        { 
            // check if hooking
            if (ropeAttached)
            {
                Unhook();
                ropeAttached = false;
            }
            
            ResetRope();
        }
    }

    private void Hook()
    {
        ropeAnchor.gameObject.SetActive(true);
        StartCoroutine(HookRoutine());
    }
    
    IEnumerator HookRoutine()
    {
        float t = 0f;
        isHooking = true;
        GetComponent<Move>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        float curSpeed = 0f;
        while (isHooking)
        {

            float acceleration = LerpClass.Lerp(t, interType);
            var direction =((Vector3)hitPos - transform.position).normalized;
            curSpeed = Mathf.Clamp(acceleration * speed, 0f, speed);
            var vel = (Vector2)direction  * curSpeed;
            GetComponent<Rigidbody2D>().velocity = vel;
            t += Time.deltaTime;
            yield return null;
        }
    }
    
    private void Unhook()
    {
        isHooking = false;
        ropeAnchor.gameObject.SetActive(false);
        hitPos = transform.position; 
        GetComponent<Move>().enabled = true;
        ResetRope();
    }
    
    private void ResetRope()
    {
        ropeAttached = false;
        ropeRenderer.positionCount = 2;
        ropeRenderer.enabled = false;
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, transform.position);
    }
    
    void OnTriggerEnter2D(Collider2D colliderStay)
    {
        if (colliderStay.gameObject.CompareTag("Anchor"))
        {
            Debug.Log("OnTriggerEnter2D");
          
            Unhook();
        }
        
    }

    private void OnTriggerExit2D(Collider2D colliderOnExit)
    {
        if (colliderOnExit.gameObject.CompareTag("Anchor"))
        {
            Debug.Log("OnTriggerExit2D");
          
        }
    }
}