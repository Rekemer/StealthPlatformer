using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EnemyCamera : MonoBehaviour, IEnemy
{
    [SerializeField] private float time;
    public float viewDistance = 4;
    private Transform playerPos;
    [SerializeField] private float viewAngleInDegrees;
    [SerializeField] private LayerMask layerMask;
    private StateMachine stateMachine = new StateMachine();
    [SerializeField] private float speed;

    private void Awake()
    {
        playerPos = FindObjectOfType<Move>().transform;
    }

    private void Start()
    {
        var patrol = new Patrol(this);
        var attack = new Attack(this);
        InitTransition(patrol, attack, CanSeePlayer);
        stateMachine.SetState(patrol);
    }

    public void Patrol()
    {
        StartCoroutine(PatrolRoutine());
    }

    IEnumerator PatrolRoutine()
    {
        float t = Mathf.InverseLerp( -viewAngleInDegrees / 2, viewAngleInDegrees / 2, transform.eulerAngles.z);
        float func;
        float angle;
        transform.eulerAngles = new Vector3(0f, 0f, -viewAngleInDegrees / 2 );
        while (stateMachine.GetCurrentState() is Patrol)
        {
            t += Time.deltaTime;
            func = Mathf.Cos(t * speed) + 1;
            angle = Mathf.LerpAngle( -viewAngleInDegrees / 2, viewAngleInDegrees / 2, func/2f);
            Debug.Log(angle) ;
            transform.eulerAngles = new Vector3(0f, 0f, angle );
            yield return null;
        }
    }

    public void Attack()
    {
        throw new NotImplementedException();
    }


    void Update()
    {
        stateMachine.Tick();
    }

    public void Rotate()
    {
        if (transform.rotation.eulerAngles.z * Mathf.Rad2Deg > 0)
        {
            iTween.RotateTo(gameObject,
                iTween.Hash("z", -viewAngleInDegrees / 2 * Mathf.Rad2Deg, "time", time, "looptype", iTween.LoopType.pingPong,
                    "easetype",
                    iTween.EaseType.linear));
        }
        else
        {
            iTween.RotateTo(gameObject,
                iTween.Hash("z", viewAngleInDegrees / 2 * Mathf.Rad2Deg, "time", time, "looptype", iTween.LoopType.pingPong,
                    "easetype",
                    iTween.EaseType.linear));
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float leftAngleInRadians = -viewAngleInDegrees / 2* Mathf.Deg2Rad - 90* Mathf.Deg2Rad;
        float rightAngleInRadians = viewAngleInDegrees / 2* Mathf.Deg2Rad - 90* Mathf.Deg2Rad ;
        var leftVector = GetVectorFromAngle(leftAngleInRadians).normalized;
        var rightVector = GetVectorFromAngle(rightAngleInRadians).normalized;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawLine(Vector3.zero, (Vector3) leftVector * viewDistance);
        Gizmos.DrawLine(Vector3.zero, (Vector3) rightVector * viewDistance);
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
        stateMachine.AddTransition(from, to, predicate);
    }
}