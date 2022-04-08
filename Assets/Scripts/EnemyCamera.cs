using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EnemyCamera : MonoBehaviour
{
    [SerializeField] private float time;
    public float viewDistance = 4;
    private Transform playerPos;
    [SerializeField] private  float viewAngle;
    [SerializeField] private LayerMask layerMask;

    private void Awake()
    {
        playerPos = FindObjectOfType<Move>().transform;
    }

    private void Start()
    {
        StartCoroutine(RotateRoutine());
    }

    void Update()
    {
    }

    public void Rotate()
    {
        StartCoroutine(RotateRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        iTween.RotateTo(gameObject, new Vector3(0, 0, -75f), time);
        while (true)
        {
            yield return null;
        }
        // while (Vector3.Distance(transform.position, destination) > 0.001f)
        // {
        //     yield return null;
        // }

        // iTween.Stop(gameObject);
        // float angle =0;
        // float t = 0;
        // while (true)
        // {
        //     var interp = LerpClass.Lerp(t, LerpClass.InterType.Linear);
        //     angle += Mathf.Lerp(0, 180, t/5f);
        //     transform.eulerAngles =new Vector3(0,0,angle);
        //     t += Time.deltaTime;
        //     yield return null;
        // }
    }

    private void OnDrawGizmos()
    {   
        Gizmos.color = Color.red;
        float leftAngle = -viewAngle / 2 - 90 * Mathf.Deg2Rad; 
        float rightAngle = viewAngle / 2- 90* Mathf.Deg2Rad;
        var leftVector = GetVectorFromAngle(leftAngle);
        var rightVector = GetVectorFromAngle(rightAngle);
        Gizmos.matrix = transform.localToWorldMatrix; 
        Gizmos.DrawLine(Vector3.zero,Vector3.zero + (Vector3)leftVector * viewDistance);
        Gizmos.DrawLine(Vector3.zero,Vector3.zero + (Vector3)rightVector * viewDistance);
       
    }

    Vector2 GetVectorFromAngle(float angle)
    {
        // conversion between Unity circle and default trigonometry circle = 90 - angle
        angle += Mathf.Abs(transform.eulerAngles.y * Mathf.Deg2Rad);
        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);
        return new Vector2(x, y);
    }
    
    public bool CanSeePlayer()
    {
        if ((transform.position - playerPos.position).sqrMagnitude < Mathf.Pow(viewDistance, 2))
        {
            Vector2 dirToPlayer = (playerPos.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.right, dirToPlayer) * Mathf.Deg2Rad;

            if (angle < viewAngle / 2f)
            {
                if (!Physics2D.Linecast(transform.position, playerPos.position, layerMask))
                {
                    return true;
                }
            }
        }

        return false;
    }
}