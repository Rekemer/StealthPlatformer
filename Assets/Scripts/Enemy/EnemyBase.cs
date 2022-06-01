using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.PlayerLoop;

namespace Enemy
{
    public abstract class EnemyBase : MonoBehaviour
    {
        [SerializeField] protected float speed;
        [SerializeField] protected float viewDistance = 4;
        [SerializeField] protected float viewAngleInDegrees;
        [SerializeField] protected LayerMask obstacleMask;
        [SerializeField] protected LayerMask visibleTargetMask;
        [SerializeField] protected Light2D _light2D;
        [SerializeField] protected float meshResolution;
        protected Transform playerPos;
        protected StateMachine stateMachine = new StateMachine();

        [SerializeField] private int edgeResolveIterations;
        public List<Transform> visibleTargets = new List<Transform>();
        public MeshFilter viewMeshFilter;
        Mesh viewMesh;
        protected bool _isViewOn = true;

        protected bool IsViewOn
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


        protected void SetAngleOfLight()
        {
            if (_light2D != null)
            {
                _light2D.pointLightOuterAngle = viewAngleInDegrees;
                _light2D.pointLightInnerAngle = viewAngleInDegrees;
                _light2D.pointLightOuterRadius = viewDistance + 0.5f;
                _light2D.pointLightInnerRadius = viewDistance;
            }
        }

        private void Update()
        {
            stateMachine.Tick();
            if (IsViewOn)
            {
                DrawFieldOfView();
            }
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
            StartCoroutine(FindTargets());
        }

        private void FindVisibleTargets()
        {
            visibleTargets.Clear();
            Collider2D[] targetsInViewRadius =
                Physics2D.OverlapCircleAll(transform.position, viewDistance, visibleTargetMask);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector2.Angle(transform.right, dirToTarget) < viewAngleInDegrees / 2)
                {
                    float dstToTarget = Vector2.Distance(transform.position, target.position);

                    if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        visibleTargets.Add(target);
                    }
                }
            }
        }

        protected void DrawAngle()
        {
            Gizmos.color = Color.red;
            float leftAngleInRadians = -viewAngleInDegrees / 2 * Mathf.Deg2Rad;
            float rightAngleInRadians = viewAngleInDegrees / 2 * Mathf.Deg2Rad;
            var leftVector = GetVectorFromAngle(leftAngleInRadians).normalized;
            var rightVector = GetVectorFromAngle(rightAngleInRadians).normalized;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawLine(Vector3.zero, (Vector3) leftVector * viewDistance);
            Gizmos.DrawLine(Vector3.zero, (Vector3) rightVector * viewDistance);
        }

        private void DrawFieldOfView()
        {
            var stepCount = Mathf.RoundToInt(viewAngleInDegrees * meshResolution);
            float stepAngleSize = viewAngleInDegrees / stepCount;
            List<Vector2> hitPoints = new List<Vector2>();
            ViewCastInfo oldInfoHit = new ViewCastInfo();

            for (int i = 0; i < stepCount; i++)
            {
                float angle = transform.eulerAngles.z - viewAngleInDegrees / 2 + stepAngleSize * i;
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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewDistance, obstacleMask);

            if (hit)
            {
                return new ViewCastInfo(true, hit.point, hit.distance, angleInRadians);
            }
            else
            {
                return new ViewCastInfo(false, transform.position + dir * viewDistance, viewDistance, angleInRadians);
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

            for (int i = 0; i < edgeResolveIterations; i++)
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

        protected bool CanSeePlayer()
        {
            if ((transform.position - playerPos.position).sqrMagnitude < Mathf.Pow(viewDistance, 2))
            {
                Vector2 dirToPlayer = (playerPos.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.right, dirToPlayer);
                if (angle < viewAngleInDegrees / 2f)
                {
                    if (!Physics2D.Linecast(transform.position, playerPos.position, visibleTargetMask))
                    {
                        if (!EventSystem.isPlayerHiding)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        protected void InitTransition(IState from, IState to, Func<bool> predicate)
        {
            stateMachine.AddTransition(from, to, predicate);
        }
    }
}