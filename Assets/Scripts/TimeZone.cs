using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D))]
public class TimeZone : MonoBehaviour
{
    [SerializeField]
    private Vector2 size;

    private BoxCollider2D collider;
    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }
    
    public Vector2 Size
    {
        get
        {
            return size;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.Pause();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.Continue();
        }
    }

    private void OnEnable()
    {
        TimeZoneManager.timeZones.Add(this);
    }

    private void OnDisable()
    {
        TimeZoneManager.timeZones.Remove(this);
    }
    private void OnValidate()
    {
        if (collider != null)
        {
            collider.size = size;
        }
    }

    private void OnDestroy()
    {
        TimeZoneManager.timeZones.Remove(this);
    }
}