using UnityEngine;

public abstract class PlatformBase : MonoBehaviour
{
    [SerializeField] protected float _speed;
    [SerializeField] private Vector3[] patrolRoute;
    public Vector3[] globalPatrolRoute { get; private set; }
    private void InitializePatrolRoute()
    {
        globalPatrolRoute = new Vector3[patrolRoute.Length];
        for (int i = 0; i < globalPatrolRoute.Length; i++)
        {
            globalPatrolRoute[i] = transform.position + patrolRoute[i];
        }
    }
    protected  void Start()
    {
        
        InitializePatrolRoute();
        
        MainFunc();
    }

    protected void OnDrawGizmos()
    {
        if (patrolRoute.Length == 0) return;
        for (int i = 0; i < patrolRoute.Length; i++)
        {
            Gizmos.color = Color.red;
            Vector3 newPos = Application.isPlaying ? globalPatrolRoute[i] : transform.position + patrolRoute[i];
            Gizmos.DrawLine(newPos - Vector3.left, newPos + Vector3.left);
            Gizmos.DrawLine(newPos - Vector3.up, newPos + Vector3.up);
        }

     
    }
    protected abstract void MainFunc();
}