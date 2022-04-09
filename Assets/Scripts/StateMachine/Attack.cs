using System;
using Enemy;
using UnityEngine;

public class Attack : IState
{
    private IEnemy _enemy;

    public void Tick()
    {
        _enemy.Attack();
    }

    public void OnExit()
    {
    }

    public Attack(IEnemy enemy)
    {
        _enemy = enemy;
    }
}