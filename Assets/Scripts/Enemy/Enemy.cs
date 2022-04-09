using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IEnemy
{
    private StateMachine stateMachine = new StateMachine();
    [SerializeField] private Vector3[] patrolRoute;

    public float defaultSpeed;
    public Vector3[] globalPatrolRoute { get; private set; }

    public float viewDistance = 4;
    private Transform playerPos;
    [SerializeField] private  float viewAngleInDegrees;
    [SerializeField] private LayerMask layerMask;
    private void Awake()
    {
        playerPos = FindObjectOfType<Move>().transform;
    }

    void Start()
    {
        globalPatrolRoute = new Vector3[patrolRoute.Length];
        for (int i = 0; i < globalPatrolRoute.Length; i++)
        {
            globalPatrolRoute[i] = transform.position + patrolRoute[i];
        }

        var patrol = new Patrol(this);
        var attack = new Attack(this);
        InitTransition(patrol,attack,CanSeePlayer);
        stateMachine.SetState(patrol);
    }

    // Update is called once per frame
    void Update()
    {
       stateMachine.Tick();
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

        //draw field of view
        float leftAngleInRadians = -viewAngleInDegrees / 2 * Mathf.Deg2Rad; 
        float rightAngleInRadians = viewAngleInDegrees / 2* Mathf.Deg2Rad;
        var leftVector = GetVectorFromAngle(leftAngleInRadians).normalized;
        var rightVector = GetVectorFromAngle(rightAngleInRadians).normalized;
        Gizmos.DrawLine(transform.position,transform.position + (Vector3)leftVector * viewDistance);
        Gizmos.DrawLine(transform.position,transform.position + (Vector3)rightVector * viewDistance);
    }

    Vector2 GetVectorFromAngle(float angle)
    {
        // conversion between Unity circle and default trigonometry circle = 90 - angle
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

            if (angle < viewAngleInDegrees / 2f)
            {
                if (!Physics2D.Linecast(transform.position, playerPos.position, layerMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void InitTransition(IState from, IState to, Func<bool> predicate)
    {
        stateMachine.AddTransition(from,to,predicate);
    }
    public void Patrol()
    {
        StartCoroutine(PatrolRoutine(globalPatrolRoute));
    }

    public void Attack()
    {
        
    }

 

    IEnumerator PatrolRoutine(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWayPointIndex = 1;

        Vector3 targetWayPoint = waypoints[0];

        TurnTo(targetWayPoint);
        while (stateMachine.GetCurrentState() is Patrol)
        {
            targetWayPoint.y = transform.position.y;
            transform.position = Vector2.MoveTowards(transform.position, targetWayPoint, defaultSpeed * Time.deltaTime);
            if (transform.position.x == targetWayPoint.x)
            {
                targetWayPointIndex = (targetWayPointIndex + 1) % waypoints.Length;
                targetWayPoint = waypoints[targetWayPointIndex];

                yield return new WaitForSeconds(1f); // to wait after reaching point

                TurnTo(targetWayPoint);
            }

            yield return null;
        }
    }

    protected void TurnTo(Vector3 whereToTurn)
    {
        var direction = (whereToTurn - transform.position).normalized;
        float dot = Vector2.Dot(transform.right, direction);
        float yAngle = dot > 0 ? 0 : 180;
        transform.Rotate(Vector3.up, yAngle);
    }
}