using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshVisualisation : MonoBehaviour
{
    [SerializeField] private VisualisationData _visualisationData;
    public List<Transform> visibleTargets = new List<Transform>();
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;
    protected bool _isViewOn = true;

    
    private void OnDrawGizmos()
    {
        DrawAngle();
    }

    
    IEnumerator FindTargets()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            FindVisibleTargets();
        }
    }

    protected void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        viewMeshFilter.GetComponent<MeshRenderer>().material = _visualisationData.materialOfVisualField; 
        
        StartCoroutine(FindTargets());
    }
    private void Update()
    {
        if (IsViewOn)
        {
            DrawFieldOfView();
        }
    }

    public bool IsViewOn
    {
        get
        {
            return _isViewOn;
        }
        set
        {
            _isViewOn = value;
            viewMeshFilter.gameObject.SetActive(_isViewOn);
        }
    }

    [SerializeField] private float edgeDstThreshold;

    
    private void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider2D[] targetsInViewRadius =
            Physics2D.OverlapCircleAll(transform.position, _visualisationData.viewDistance, _visualisationData.visibleTargetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector2.Angle(transform.right, dirToTarget) < _visualisationData.viewAngleInDegrees / 2)
            {
                float dstToTarget = Vector2.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, _visualisationData.obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }
    
     protected void DrawAngle()
        {
            Gizmos.color = Color.red;
            float leftAngleInRadians = -_visualisationData.viewAngleInDegrees / 2 * Mathf.Deg2Rad;
            float rightAngleInRadians = _visualisationData.viewAngleInDegrees / 2 * Mathf.Deg2Rad;
            var leftVector = GetVectorFromAngle(leftAngleInRadians).normalized;
            var rightVector = GetVectorFromAngle(rightAngleInRadians).normalized;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawLine(Vector3.zero, (Vector3) leftVector * _visualisationData.viewDistance);
            Gizmos.DrawLine(Vector3.zero, (Vector3) rightVector * _visualisationData.viewDistance);
        }

        private void DrawFieldOfView()
        {
            float leftAngleInRadians = -_visualisationData.viewAngleInDegrees / 2 * Mathf.Deg2Rad;
            float rightAngleInRadians = _visualisationData.viewAngleInDegrees / 2 * Mathf.Deg2Rad;
           
            var stepCount = Mathf.RoundToInt(_visualisationData.viewAngleInDegrees * _visualisationData.meshResolution);
            float stepAngleSize = _visualisationData.viewAngleInDegrees / stepCount;
          
            List<Vector2> hitPoints = new List<Vector2>();
            ViewCastInfo oldInfoHit = new ViewCastInfo();

            
            
           
            
            for (int i = 0; i < stepCount; i++)
            {
                float angle = transform.eulerAngles.z  - _visualisationData.viewAngleInDegrees / 2 + stepAngleSize * i;
               
                 angle = Mathf.Clamp(angle, transform.eulerAngles.z - _visualisationData.viewAngleInDegrees / 2,
                     transform.eulerAngles.z + _visualisationData.viewAngleInDegrees / 2);
                angle *= Mathf.Deg2Rad;
                // var vector = (Vector3) GetVectorFromAngle(angle);
                var infoHit = GetHit(angle);
                if (i > 0)
                {
                    bool edgeDstThresholdExceeded = Mathf.Abs(oldInfoHit.dst - infoHit.dst) > edgeDstThreshold;
                    if (oldInfoHit.hit != infoHit.hit || (oldInfoHit.hit && infoHit.hit && edgeDstThresholdExceeded))
                    {
                        EdgeInfo edge = FindEdge(oldInfoHit, infoHit);
                        if (edge.pointA != Vector3.zero)
                        {
                            hitPoints.Add(edge.pointA);
                        }

                        if (edge.pointB != Vector3.zero)
                        {
                            hitPoints.Add(edge.pointB);
                        }
                    }
                }

                Debug.DrawLine(transform.position, infoHit.point);
                hitPoints.Add(infoHit.point);
                oldInfoHit = infoHit;
            }

            int vertexCount = hitPoints.Count + 1;
            var vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];
            vertices[0] = Vector3.zero;
            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(hitPoints[i]);

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            viewMesh.Clear();

            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        }

        ViewCastInfo GetHit(float angleInRadians)
        {
            Vector3 dir = GetVectorFromAngle(angleInRadians);
            var dot =Vector3.Dot(dir, transform.right);
             dir.x *= Mathf.Sign(dot);
           // Debug.Log(transform.right.y);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, _visualisationData.viewDistance, _visualisationData.obstacleMask);

            if (hit)
            {
                return new ViewCastInfo(true, hit.point, hit.distance, angleInRadians);
            }
            else
            {
                return new ViewCastInfo(false, transform.position + dir * _visualisationData.viewDistance, _visualisationData.viewDistance, angleInRadians);
            }
        }

        public struct ViewCastInfo
        {
            public bool hit;
            public Vector3 point;
            public float dst;
            public float angle;

            public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
            {
                hit = _hit;
                point = _point;
                dst = _dst;
                angle = _angle;
            }
        }

        public struct EdgeInfo
        {
            public Vector3 pointA;
            public Vector3 pointB;

            public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
            {
                pointA = _pointA;
                pointB = _pointB;
            }
        }

        EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            float minAngle = minViewCast.angle;
            float maxAngle = maxViewCast.angle;
            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            for (int i = 0; i < _visualisationData.edgeResolveIterations; i++)
            {
                float angle = (minAngle + maxAngle) / 2;
                ViewCastInfo newViewCast = GetHit(angle);

                bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.point;
                }
            }

            return new EdgeInfo(minPoint, maxPoint);
        }


        protected Vector2 GetVectorFromAngle(float angle)
        {
            // conversion between Unity circle and default trigonometry circle = 90 - angle
            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);
            return new Vector2(x, y);
        }
}
