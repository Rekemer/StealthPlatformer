#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteAlways]
    public class Detector : MonoBehaviour, IHaveSize
    {
        public Vector2 Size { get => size; }
        [SerializeField]
        private Vector2 size;
        private BoxCollider2D collider;
        private void Awake()
        {
            collider = GetComponent<BoxCollider2D>();
        }

        private void OnEnable()
        {
            if (!DetectorManager.objects.Contains(this))
            {
                DetectorManager.objects.Add(this);
            }
        }

        private void OnDisable()
        {
            if (DetectorManager.objects.Contains(this))
            {
                DetectorManager.objects.Remove(this);
            }
        }
        private void OnValidate() // when something in inspector changes this function is called
        {
            if (collider != null)
            {
                collider.size = size;
            }
        }
    }
