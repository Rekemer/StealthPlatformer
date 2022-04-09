using System;
using System.Collections;
using UnityEngine;

public class Patrol : IState
{
    private IEnumerator currentRoutine;
    private IEnemy _enemy;
    private bool isFollowing;
    public Patrol(IEnemy enemy) 
    {
        _enemy = enemy;
    }

    public  void Tick()
    {
        if (!isFollowing)
        {
            _enemy.Patrol();
            isFollowing = true;
        }
        
    }

    public  void OnExit()
    {
        
    }

    
    
}