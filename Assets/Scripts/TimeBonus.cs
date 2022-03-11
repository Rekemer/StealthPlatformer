using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class TimeBonus : MonoBehaviour
{
    [SerializeField] private TimeValue timeValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddTime(timeValue.time);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
