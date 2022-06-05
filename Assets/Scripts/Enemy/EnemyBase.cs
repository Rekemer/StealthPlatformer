using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.PlayerLoop;

namespace Enemy
{
    public abstract class EnemyBase : MonoBehaviour
    {
        [SerializeField] protected float speed;
        protected MeshVisualisation _meshVisualisation;
        protected Transform playerPos;
        protected StateMachine stateMachine = new StateMachine();

        protected void Awake()
        {
            _meshVisualisation = GetComponent<MeshVisualisation>();
        }


        protected void SetAngleOfLight()
        {
           
        }

        private void Update()
        {
            stateMachine.Tick();
            
        }

        

        

       

        protected bool CanSeePlayer()
        {
            if ((transform.position - playerPos.position).sqrMagnitude < Mathf.Pow(_meshVisualisation.ViewDistance, 2))
            {
                Vector2 dirToPlayer = (playerPos.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.right, dirToPlayer);
                if (angle < _meshVisualisation.ViewAngleInDegrees / 2f)
                {
                    RaycastHit2D hit2D = Physics2D.Linecast(transform.position, playerPos.position,
                        _meshVisualisation.ObstacleMask);
                    bool isPlayer = hit2D.transform.GetComponent<PlayerMove>();
                    if (isPlayer )
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