using UnityEngine;

public abstract class State 
{
    protected Enemy enemy;

    protected State(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public abstract void Tick();
    public abstract void OnExit();
}