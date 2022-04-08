using System;
using UnityEngine;

public class Attack : State
{
    public override void Tick()
    {
        Debug.Log("Player is Attacked");
    }

    public override void OnExit()
    {
        
    }

    public Attack(Enemy enemy) : base(enemy)
    {
    }
}