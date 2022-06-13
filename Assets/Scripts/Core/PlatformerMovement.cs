using System;
using Environment;
using UnityEngine;

namespace Core
{
    public class PlatformerMovement : MonoBehaviour
    {
        private Rigidbody2D platformRBody;
        private bool _isOnPlatform;
        private Rigidbody2D rbody;
        private void Awake()
        {
            rbody = GetComponent<Rigidbody2D>();
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if(col.gameObject.tag == "Platform")
            {
                if (!col.gameObject.GetComponent<PlungingPlatform>().IsPlunging)
                {
                    col.gameObject.GetComponent<PlungingPlatform>().Plunging();
                }
                
                platformRBody = col.gameObject.GetComponent<Rigidbody2D>();
                _isOnPlatform = true;
            }
        }
 
        void OnCollisionExit2D(Collision2D col)
        {
            if(col.gameObject.tag == "Platform")
            {
               
                _isOnPlatform = false;
                platformRBody = null;
            }
        }
        void FixedUpdate()
        {
            if(_isOnPlatform)
            {
                rbody.velocity = rbody.velocity + platformRBody.velocity;
            }
        }
    }
}