using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Platform : PlatformBase
{
    protected override void MainFunc()
    {
        Patrol();
    }


    public void Patrol()
    {
        StartCoroutine(PatrolRoutine(globalPatrolRoute));
    }
    
    
   
    
    IEnumerator PatrolRoutine(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWayPointIndex = 1;

        Vector3 targetWayPoint = waypoints[targetWayPointIndex];

        
        while (true)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetWayPoint, _speed * Time.deltaTime);
            if (transform.position == targetWayPoint)
            {
                targetWayPointIndex = (targetWayPointIndex + 1) % waypoints.Length;
                targetWayPoint = waypoints[targetWayPointIndex];

                yield return new WaitForSeconds(1f); // to wait after reaching point

                
            }

            yield return null;
        }
    }
    
    
    
}
