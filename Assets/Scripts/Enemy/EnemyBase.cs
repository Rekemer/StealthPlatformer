using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.PlayerLoop;

namespace Enemy
{
    public abstract class EnemyBase : MonoBehaviour
    {
        [SerializeField] protected float speed;
        [SerializeField] protected float viewDistance = 4;
        [SerializeField] protected float viewAngleInDegrees;
        [SerializeField] protected LayerMask obstacleMask;
        [SerializeField] protected LayerMask visibleTargetMask;
        [SerializeField] protected Light2D _light2D;
       
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

        private void Update()
        {
            stateMachine.Tick();
            
        }

        

        

       

        protected bool CanSeePlayer()
        {
            if ((transform.position - playerPos.position).sqrMagnitude < Mathf.Pow(viewDistance, 2))
            {
                Vector2 dirToPlayer = (playerPos.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.right, dirToPlayer);
                if (angle < viewAngleInDegrees / 2f)
                {
                    if (!Physics2D.Linecast(transform.position, playerPos.position, visibleTargetMask))
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
    }
}