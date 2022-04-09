using System;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class EnemyCamera : EnemyBase, IEnemy
    {
      

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
            while (stateMachine.GetCurrentState() is Patrol)
            {
                t += Time.deltaTime;
                func = Mathf.Cos(t * speed) + 1;
                angle = Mathf.LerpAngle( -viewAngleInDegrees / 2, viewAngleInDegrees / 2, func/2f);
                transform.localEulerAngles = new Vector3(0f, 0f, angle );
                yield return null;
            }
        }
        private void OnDrawGizmos()
        {
            DrawAngle();
        }

        

        public void Attack()
        {
            Debug.Log("CAMERA ENEMY: gotcha! ");
        }


        void Update()
        {
            stateMachine.Tick();
        }
    

      
    }
}