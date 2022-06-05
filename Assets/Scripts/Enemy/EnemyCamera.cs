using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Enemy
{
    public class EnemyCamera : EnemyBase, IEnemy
    {
        [SerializeField] private float _waitTime;
        [SerializeField] private Beep _beep;
        [SerializeField] private AIType _aiType;
        private bool _isTurnedOff;
        private bool _isSwitched;
        private void Awake()
        {
            base.Awake();
            playerPos = FindObjectOfType<PlayerMove>().transform;
        }

        private void Start()
        {
          
            if (_aiType == AIType.Patrol)
            {
                var patrol = new Patrol(this);
                var attack = new Attack(this);
                InitTransition(patrol, attack, CanSeePlayer);
                stateMachine.SetState(patrol);
            }
            else if (_aiType == AIType.Switch)
            {
                var swit = new Switch(this); 
                var attack = new Attack(this);
                Func<bool> HasTarget() => () => CanSeePlayer() && !_isTurnedOff;
                InitTransition(swit, attack, HasTarget());
                stateMachine.SetState(swit);
            }
            SetAngleOfLight();
        }
        
        private void OnValidate()
        {
            SetAngleOfLight();
        }

        
        
        public void Patrol()
        {
            StartCoroutine(PatrolRoutine());
        }

        IEnumerator PatrolRoutine()
        {
            var initAngle = transform.eulerAngles.z;
            float viewAngleInDegrees =_meshVisualisation.ViewAngleInDegrees;
            float t = Mathf.InverseLerp( initAngle-  viewAngleInDegrees / 2, initAngle+ viewAngleInDegrees / 2, transform.eulerAngles.z);
            float func;
            float angle;
            while (stateMachine.GetCurrentState() is Patrol)
            {
                t += Time.deltaTime;
                func = Mathf.Cos(t * speed) + 1;
                angle = Mathf.LerpAngle( initAngle-viewAngleInDegrees / 2, initAngle+viewAngleInDegrees / 2, func/2f);
                transform.localEulerAngles = new Vector3(0f, 0f, angle );
                yield return null;
            }
        }
       
        

        public void Attack()
        {
            Debug.Log("CAMERA ENEMY: gotcha! ");
            EventSystem.current.OnSpotted();
        }

        public void Switch()
        {
            if (!_isSwitched)
            {
                StartCoroutine(SwitchRoutine());
                Debug.Log("Switch");
            }
           
        }

        private IEnumerator SwitchRoutine()
        {
            _isSwitched = true;
            _beep.IsTurnedOff = _isTurnedOff = true;
            GetComponent<MeshVisualisation>().IsViewOn = false;
           // _light2D.enabled = false;
            yield return new WaitForSeconds(_waitTime);
            //_light2D.enabled = true;
            GetComponent<MeshVisualisation>().IsViewOn = true;
            _beep.IsTurnedOff = false;
            _isTurnedOff = false;
            yield return new WaitForSeconds(_waitTime);
            _isSwitched = false;
            
        }

        
    

      
    }
}