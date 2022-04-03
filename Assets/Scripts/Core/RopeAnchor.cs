using System;
using UnityEngine;

namespace Core
{
    public class RopeAnchor : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D collider;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            collider = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            collider.isTrigger = true;
        }

      
    }
}