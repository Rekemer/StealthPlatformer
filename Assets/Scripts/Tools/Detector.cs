#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

[ExecuteAlways]
public class Detector : MonoBehaviour, IHaveSize
{
    public Vector2 Size
    {
        get
        {
#if UNITY_EDITOR
            float playerScale = transform.parent.parent.localScale.x;
            size = _size * playerScale;
            return size;
#else
            size = _size;
            return size;
#endif
        }
    }


    private Vector2 size;
    [SerializeField] private Vector2 _size;
    private BoxCollider2D collider;

    private void Awake()
    {
        float playerScale = transform.parent.parent.localScale.x;
        size = _size * playerScale;
    }

    private void
        OnValidate() // when something in inspector changes this function is called - change size of collider from script inspector field
    {
        float playerScale = transform.parent.parent.localScale.x;
#if UNITY_EDITOR
        size = _size * playerScale;
#else
            size = _size;
#endif
    }
}