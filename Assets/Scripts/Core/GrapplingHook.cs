using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    // in what way we getting closer to hook 
    [SerializeField] private LerpClass.InterType interType;
    [SerializeField] private float speed;
    [SerializeField] private float ropeMaxCastDistance;
    [SerializeField] private SpriteRenderer ropeHingeAnchorSprite;
    public SpriteRenderer crosshairSprite;
    public Transform crosshair;
    public LineRenderer ropeRenderer;
    public LayerMask ropeLayerMask;
    
    private bool ropeAttached;
    private bool isColliding;
    private bool isHooking;
    private Vector2 hitPos;
    // Start is called before the first frame update
    void Start()
    {
        ropeRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        var worldMousePosition =
            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
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
        StartCoroutine(HookRoutine());
    }

    IEnumerator HookRoutine()
    {
        float t = 0f;
        isHooking = true;
        GetComponent<Move>().enabled = false;
        while (isHooking)
        {

            float acceleration = LerpClass.Lerp(t, interType);
            var direction =((Vector3)hitPos - transform.position).normalized;
            var vel = acceleration * (Vector2)direction + GetComponent<Rigidbody2D>().velocity;
            GetComponent<Rigidbody2D>().velocity= vel;
            t += Time.deltaTime;
            yield return null;
        }
        
       
    }
    
    private void Unhook()
    {
        isHooking = false;
        GetComponent<Move>().enabled = true;
    }
    
    private void ResetRope()
    {
        ropeAttached = false;

        ropeRenderer.positionCount = 2;
        ropeRenderer.enabled = false;
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, transform.position);

        ropeHingeAnchorSprite.enabled = false;
    }
    
    void OnTriggerStay2D(Collider2D colliderStay)
    {
        isColliding = true;
    }

    private void OnTriggerExit2D(Collider2D colliderOnExit)
    {
        isColliding = false;
    }
}