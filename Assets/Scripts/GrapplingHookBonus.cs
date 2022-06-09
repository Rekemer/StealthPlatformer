using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookBonus : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem vfx;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var hook = other.GetComponent<GrapplingHook>();
            if (hook != null)
            {
                EventSystem.current.ActivateBonus();
                hook.GotBonus = true;
                vfx.Play();
            }
        }
    }
}
