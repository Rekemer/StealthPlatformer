using System.Collections;
using UnityEngine;

namespace Environment
{
    public class PlungingPlatform : PlatformBase
    {
        [SerializeField] private LerpClass.InterType InterTypeToPlunge;
        [SerializeField] private LerpClass.InterType InterTypeToGoBack;
        [SerializeField] private float timeToWaitAfterPlunging;
        private bool IsActivated;
       
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // start plunging
                if (!IsActivated)
                {
                    Debug.Log("Plunging");
                    Plunging();
                }
            }
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // start plunging
                if (!IsActivated)
                {
                    Debug.Log("PlungingMainFunc");
                    Plunging();
                }
            }
        }

        private void Plunging()
        {
            StartCoroutine(PlungingRoutine());
        }

        private IEnumerator PlungingRoutine()
        {
            IsActivated = true;
            // start plunging
            float elapsedTime = 0;
            while (true)
            {
                Debug.Log("Plunging");
                var state = Vector3.Distance(transform.position, globalPatrolRoute[1]) < 0.1;
                if (state) break; 
                elapsedTime += Time.deltaTime;
                var t = LerpClass.Lerp(elapsedTime, InterTypeToPlunge);
                transform.position = Vector2.Lerp(globalPatrolRoute[0], globalPatrolRoute[1], t);
                yield return null;
            }

            yield return new WaitForSeconds(timeToWaitAfterPlunging);
            elapsedTime = 0;
            // return to previous position
            while (true)
            {
                Debug.Log("Ascending");
                var state = Vector3.Distance(transform.position, globalPatrolRoute[0]) < float.Epsilon;
                if (state) break; 
                elapsedTime += Time.deltaTime;
                var t = LerpClass.Lerp(elapsedTime, InterTypeToGoBack);
                transform.position = Vector2.Lerp(globalPatrolRoute[1], globalPatrolRoute[0], t);
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