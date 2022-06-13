using System;
using System.Collections;
using UnityEngine;

namespace Environment
{
    public class PlungingPlatform : PlatformBase
    {
        [SerializeField] private LerpClass.InterType InterTypeToPlunge;
        [SerializeField] private LerpClass.InterType InterTypeToGoBack;
        [SerializeField] private float timeToWaitAfterPlunging;
        [SerializeField] private float timeToWaitBeforePlunging;
        private bool IsActivated;
        private Rigidbody2D rb;

        public bool IsPlunging => IsActivated;
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Plunging()
        {
            if (!IsActivated)
            {
                StartCoroutine(PlungingRoutine());
            }
           
        }

        private IEnumerator PlungingRoutine()
        {
            yield return new WaitForSeconds(timeToWaitBeforePlunging);
            IsActivated = true;
            transform.position = globalPatrolRoute[0];
            Vector2 startPosition = transform.position;
            Vector2 targetWayPoint = globalPatrolRoute[1];
            // start plunging
            float elapsedTime = 0;
            while (true)
            {
               // Debug.Log("Plunging");
                var state = Vector3.Distance(transform.position, globalPatrolRoute[1]) < 0.1f;
                if (state) break; 
                elapsedTime += Time.deltaTime;
                var t = LerpClass.Lerp(elapsedTime, InterTypeToPlunge);
                rb.MovePosition(Vector2.Lerp(startPosition, targetWayPoint, t));
                yield return null;
            }

            yield return new WaitForSeconds(timeToWaitAfterPlunging);
            elapsedTime = 0;
            targetWayPoint = globalPatrolRoute[0];
            startPosition =globalPatrolRoute[1];
            // return to previous position
            while (true)
            {
               // Debug.Log("Ascending");
               var state = Vector3.Distance(transform.position, globalPatrolRoute[0]) < 0.1f;
               if (state) break; 
               elapsedTime += Time.deltaTime;
               var t = LerpClass.Lerp(elapsedTime, InterTypeToPlunge);
               rb.MovePosition(Vector2.Lerp(startPosition, targetWayPoint, t));
                yield return null;
            }
            IsActivated = false;
            
        }

        protected override void MainFunc()
        {
            if (globalPatrolRoute.Length != 2)
            {
                Debug.LogWarning("plunging platform has 2 points to work with! ");
            }

            transform.position = globalPatrolRoute[0];
        }
        
        
        
        
    }
}