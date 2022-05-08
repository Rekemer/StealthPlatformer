using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Vector3[] patrolRoute;
    public Vector3[] globalPatrolRoute { get; private set; }
    private void Start()
    {
        globalPatrolRoute = new Vector3[patrolRoute.Length];
        for (int i = 0; i < globalPatrolRoute.Length; i++)
        {
            globalPatrolRoute[i] = transform.position + patrolRoute[i];
        }
        Patrol();
    }

    public void Patrol()
    {
        StartCoroutine(PatrolRoutine(globalPatrolRoute));
    }
    private void OnDrawGizmos()
    {
        if (patrolRoute.Length == 0) return;
        for (int i = 0; i < patrolRoute.Length; i++)
        {
            Gizmos.color = Color.red;
            Vector3 newPos = Application.isPlaying ? globalPatrolRoute[i] : transform.position + patrolRoute[i];
            Gizmos.DrawLine(newPos - Vector3.left, newPos + Vector3.left);
            Gizmos.DrawLine(newPos - Vector3.up, newPos + Vector3.up);
        }

     
    }
    
   
    
    IEnumerator PatrolRoutine(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWayPointIndex = 1;

        Vector3 targetWayPoint = waypoints[0];

        
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
