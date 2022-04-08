using System;
using System.Collections;
using UnityEngine;

public class Idle : State
{
    private IEnumerator currentRoutine;
    private bool isFollowing;
    public Idle(Enemy enemy) : base(enemy)
    {
    }

    public override void Tick()
    {
        if (!isFollowing)
        {
            enemy.FollowRoute();
            isFollowing = true;
        }
        
    }

    public override void OnExit()
    {
       
    }

    
    
}