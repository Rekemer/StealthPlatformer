
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D))]
public class TimeZone : MonoBehaviour, IHaveSize
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
        TimeZoneManager.objects.Add(this);
    }

    private void OnDisable()
    {
        TimeZoneManager.objects.Remove(this);
    }
    private void OnValidate() // when something in inspector changes this function is called
    {
        if (collider != null)
        {
            collider.size = size;
        }
    }

    private void OnDestroy()
    {
        TimeZoneManager.objects.Remove(this);
    }
}