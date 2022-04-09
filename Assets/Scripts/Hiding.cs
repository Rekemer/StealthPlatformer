
    using System;
    using UnityEngine;

    public class Hiding: MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                EventSystem.isPlayerHiding = true;
            }
            
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                EventSystem.isPlayerHiding = false;
            }
        }
    }
