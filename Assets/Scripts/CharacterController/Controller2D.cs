using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    private const float skinWidth = .015f;
    private BoxCollider2D collider;
    private RaycastsOrigin raycasts;

    [SerializeField] int verticalRayAmount = 2;
    [SerializeField] int horizontalRayAmount = 2;
    [SerializeField] private LayerMask maskCollision;
    private float raySpacingHorizontal;
    private float raySpacingVertical;
    private float verticalSpacing;
    private float horizontalSpacing;
   

    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        CalculateRaySpacing();
    }

    void Update()
    {
        CalculateRayOrigins();

        for (int i = 0; i < verticalRayAmount; i++)
        {
            Debug.DrawRay(raycasts.bottomLeft + new Vector2(i * verticalSpacing, 0), Vector3.up * -4f,
                Color.red);
        }
    }

    public void Move(Vector2 velocity)
    {
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
    }

    void VerticalCollisions(ref Vector2 velocity)
    {
        var directionY = Mathf.Sign(velocity.y);

        var rayLength = Mathf.Abs(velocity.y) + skinWidth;
        for (int i = 0; i < verticalRayAmount; i++)
        {
            var rayOrigin = directionY == 1 ? raycasts.topLeft : raycasts.bottomLeft;
            rayOrigin += Vector2.right * (verticalSpacing * i + velocity.x);
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, directionY * Vector2.up, rayLength, maskCollision);
            if (hit2D)
            {
                velocity.y = directionY * (hit2D.distance);
                rayLength = hit2D.distance;
            }
        }
    }

    struct RaycastsOrigin
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }

    void CalculateRayOrigins()
    {
        var bounds = collider.bounds;
        bounds.Expand(skinWidth * -2f);
        raycasts.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycasts.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycasts.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycasts.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    private void CalculateRaySpacing()
    {
        var bounds = collider.bounds;
        bounds.Expand(skinWidth * -2f);
        verticalRayAmount = Mathf.Clamp(verticalRayAmount, 2, Int32.MaxValue);
        horizontalRayAmount = Mathf.Clamp(horizontalRayAmount, 2, Int32.MaxValue);
        verticalSpacing = bounds.size.x / (verticalRayAmount - 1);
        horizontalSpacing = bounds.size.y / (horizontalRayAmount - 1);
    }

    // Start is called before the first frame update


    // Update is called once per frame
}