using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

/*namespace Enemy
{
    public abstract class EnemyBase : MonoBehaviour
    {
        [SerializeField] protected float speed;
        [SerializeField] protected float viewDistance = 4;
        [SerializeField] protected float viewAngleInDegrees;
        [SerializeField] protected LayerMask layerMask; 
        [SerializeField] protected  Light2D _light2D;
        protected Transform playerPos;
        protected StateMachine stateMachine = new StateMachine();
        
        protected void SetAngleOfLight()
        {
            if (_light2D != null)
            {
                _light2D.pointLightOuterAngle = viewAngleInDegrees;
                _light2D.pointLightInnerAngle = viewAngleInDegrees;
                _light2D.pointLightOuterRadius = viewDistance + 0.5f;
                _light2D.pointLightInnerRadius = viewDistance;
            }
        }
        protected Vector2 GetVectorFromAngle(float angle)
        {
            // conversion between Unity circle and default trigonometry circle = 90 - angle
            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);
            return new Vector2(x, y);
        }

        protected bool CanSeePlayer()
        {
            if ((transform.position - playerPos.position).sqrMagnitude < Mathf.Pow(viewDistance, 2))
            {
                Vector2 dirToPlayer = (playerPos.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.right, dirToPlayer);
                if (angle < viewAngleInDegrees / 2f)
                {
                    if (!Physics2D.Linecast(transform.position, playerPos.position, layerMask))
                    {
                        if (!EventSystem.isPlayerHiding)
                        {
                            return true;
                        }
                       
                    }
                }
            }

            return false;
        }

        protected void InitTransition(IState from, IState to, Func<bool> predicate)
        {
            stateMachine.AddTransition(from, to, predicate);
        }
        protected void DrawAngle()
        {
            Gizmos.color = Color.red;
            float leftAngleInRadians = -viewAngleInDegrees / 2 * Mathf.Deg2Rad;
            float rightAngleInRadians = viewAngleInDegrees / 2 * Mathf.Deg2Rad;
            var leftVector = GetVectorFromAngle(leftAngleInRadians).normalized;
            var rightVector = GetVectorFromAngle(rightAngleInRadians).normalized;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawLine(Vector3.zero, (Vector3) leftVector * viewDistance);
            Gizmos.DrawLine(Vector3.zero, (Vector3) rightVector * viewDistance);
        }
    }
}*/