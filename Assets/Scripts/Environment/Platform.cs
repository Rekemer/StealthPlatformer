using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Platform : PlatformBase
{
    private Rigidbody2D rb;
    
    protected override void MainFunc()
    {
        Patrol();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Patrol()
    {
        StartCoroutine(PatrolRoutine(globalPatrolRoute));
    }
    
    
   
    
    IEnumerator PatrolRoutine(Vector3[] waypoints)
    {
        transform.position = waypoints[0];
        var startPosition = transform.position;
        int targetWayPointIndex = 1;

        Vector3 targetWayPoint = waypoints[targetWayPointIndex];

        float time = 0;
        while (true)
        {
            time += Time.deltaTime;
            var t = (time/Vector2.Distance(startPosition, targetWayPoint))*_speed;
            rb.MovePosition(Vector2.Lerp(startPosition, targetWayPoint, t));
            
            if ((transform.position - targetWayPoint).magnitude <= 0.1f)
            {
                startPosition = targetWayPoint;
                targetWayPointIndex = (targetWayPointIndex + 1) % waypoints.Length;
                targetWayPoint = waypoints[targetWayPointIndex];
                time = 0;
                yield return new WaitForSeconds(1f); // to wait after reaching point
            }

            yield return null;
        }
    }
    
    
    
}
