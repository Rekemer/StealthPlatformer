using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookBonus : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var hook = other.GetComponent<GrapplingHook>();
            if (hook != null)
            {
                hook.ResetHook(true);
            }
        }
    }
}
