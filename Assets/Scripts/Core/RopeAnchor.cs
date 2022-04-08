using System;
using UnityEngine;

namespace Core
{
    public class RopeAnchor : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _collider;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            _collider.isTrigger = true;
        }

      
    }
}