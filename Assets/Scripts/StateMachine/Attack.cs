using System;
using UnityEngine;

public class Attack : IState
{
    private IEnemy _enemy;

    public void Tick()
    {
        Debug.Log("Player is Attacked");
    }

    public void OnExit()
    {
    }

    public Attack(IEnemy enemy)
    {
        _enemy = enemy;
    }
}