using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Enemy
{
    public class Enemy : EnemyBase, IEnemy
    {
        [SerializeField] private Vector3[] patrolRoute;

        public Vector3[] globalPatrolRoute { get; private set; }

       

        private void Awake()
        {
            playerPos = FindObjectOfType<Move>().transform;
        }

        void Start()
        {
            base.Start();
            globalPatrolRoute = new Vector3[patrolRoute.Length];
            for (int i = 0; i < globalPatrolRoute.Length; i++)
            {
                globalPatrolRoute[i] = transform.position + patrolRoute[i];
            }

            var patrol = new Patrol(this);
            var attack = new Attack(this);
            InitTransition(patrol, attack, CanSeePlayer);
            stateMachine.SetState(patrol);
        }

        // Update is called once per frame
      

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

            DrawAngle();
        }

        private void OnValidate()
        {
            SetAngleOfLight();
        }
        
        void SetAngleOfLight()
        {
            if (_light2D != null)
            {
                _light2D.pointLightOuterAngle = viewAngleInDegrees;
                _light2D.pointLightInnerAngle = viewAngleInDegrees;
                _light2D.pointLightOuterRadius = viewDistance + 0.1f;
                _light2D.pointLightInnerRadius = viewDistance;
            }
        }
        
        public void Patrol()
        {
            StartCoroutine(PatrolRoutine(globalPatrolRoute));
        }

        public void Attack()
        {
            Debug.Log("ENEMY WALKING: gotcha! ");
            GameManager.Instance.IsGameOver = true;
        }

        public void Switch()
        {
            throw new NotImplementedException();
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
                transform.position = Vector2.MoveTowards(transform.position, targetWayPoint, speed * Time.deltaTime);
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

        void TurnTo(Vector3 whereToTurn)
        {
            var direction = (whereToTurn - transform.position).normalized;
            float dot = Vector2.Dot(transform.right, direction);
            float yAngle = dot > 0 ? 0 : 180;
            transform.Rotate(Vector3.up, yAngle); 
            //viewAngleInDegrees += 180;
        }
    }
}